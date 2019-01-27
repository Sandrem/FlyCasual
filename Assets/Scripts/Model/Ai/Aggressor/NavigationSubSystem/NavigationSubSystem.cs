using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BoardTools;
using Movement;
using Ship;
using UnityEngine;

namespace AI.Aggressor
{
    public static class NavigationSubSystem
    {
        private static GenericShip CurrentShip;

        private static Dictionary<string, MovementComplexity> AllManeuvers;
        private static Dictionary<string, NavigationResult> NavigationResults;
        private static MovementPrediction CurrentMovementPrediction;

        private static List<NavigationResult> NextTurnNavigationResults;
        private static MovementPrediction CurrentTurnMovementPrediction;

        private static VirtualBoard VirtualBoard;

        public static string BestManeuver { get; private set; }

        public static void CalculateNavigation(GenericShip ship, Action callback)
        {
            CurrentShip = ship;

            // TODO: for each player
            VirtualBoard = new VirtualBoard();

            AllManeuvers = CurrentShip.GetManeuvers();
            NavigationResults = new Dictionary<string, NavigationResult>();

            GameManagerScript.Instance.StartCoroutine(StartCalculations(callback));
        }

        private static IEnumerator StartCalculations(Action callback)
        {
            yield return PredictManeuversOfEnemyShips();
            yield return PredictManeuversOfThisShip();
            FinishManeuverPredicition();

            callback();
        }

        private static IEnumerator PredictManeuversOfEnemyShips()
        {
            foreach (GenericShip enemyShip in CurrentShip.Owner.EnemyShips.Values)
            {
                yield return PredictManeuverOfEnemyShip(enemyShip);
                ShipPositionInfo shipPositionInfo = new ShipPositionInfo(CurrentMovementPrediction.FinalPosition, CurrentMovementPrediction.FinalAngles);
                VirtualBoard.SetVirtualPositionInfo(enemyShip, shipPositionInfo);
            }
        }

        private static IEnumerator PredictManeuverOfEnemyShip(GenericShip enemyShip)
        {
            Selection.ChangeActiveShip(enemyShip);

            GenericMovement savedMovement = enemyShip.AssignedManeuver;

            GenericMovement movement = ShipMovementScript.MovementFromString("2.F.S");
            enemyShip.SetAssignedManeuver(movement);

            CurrentMovementPrediction = new MovementPrediction(movement);
            yield return CurrentMovementPrediction.CalculateMovementPredicition();

            enemyShip.SetAssignedManeuver(savedMovement);
        }

        private static IEnumerator PredictManeuversOfThisShip()
        {
            Selection.ChangeActiveShip(CurrentShip);

            foreach (var maneuver in AllManeuvers)
            {
                GenericMovement movement = ShipMovementScript.MovementFromString(maneuver.Key);
                CurrentShip.SetAssignedManeuver(movement);
                movement.Initialize();
                movement.IsSimple = true;

                CurrentMovementPrediction = new MovementPrediction(movement);
                yield return CurrentMovementPrediction.CalculateMovementPredicition();

                VirtualBoard.SetVirtualPositionInfo(CurrentShip, new ShipPositionInfo(CurrentMovementPrediction.FinalPosition, CurrentMovementPrediction.FinalAngles));
                yield return CheckNextTurnRecursive(GetShortestTurnManeuvers());

                ProcessMovementPredicition();

                VirtualBoard.RestorePositionInfo(CurrentShip);
            }
        }

        private static IEnumerator CheckNextTurnRecursive(List<string> turnManeuvers)
        {
            NextTurnNavigationResults = new List<NavigationResult>();

            foreach (string turnManeuver in turnManeuvers)
            {
                GenericMovement movement = ShipMovementScript.MovementFromString(turnManeuver);
                CurrentShip.SetAssignedManeuver(movement);
                movement.Initialize();
                movement.IsSimple = true;
                CurrentTurnMovementPrediction = new MovementPrediction(movement);
                yield return CurrentTurnMovementPrediction.CalculateMovementPredicition();

                NextTurnNavigationResults.Add(new NavigationResult()
                {
                    isOffTheBoard = CurrentTurnMovementPrediction.IsOffTheBoard,
                    obstaclesHit = CurrentMovementPrediction.AsteroidsHit.Count
                });
            }
        }

        private static List<string> GetShortestTurnManeuvers()
        {
            List<string> bestTurnManeuvers = new List<string>();

            ManeuverHolder bestTurnManeuver = Selection.ThisShip.GetManeuverHolders()
                .Where(n =>
                    n.Bearing == ManeuverBearing.Turn
                    && n.Direction == ManeuverDirection.Left
                )
                .OrderBy(n => n.SpeedInt)
                .FirstOrDefault();
            bestTurnManeuvers.Add(bestTurnManeuver.ToString());

            bestTurnManeuver = Selection.ThisShip.GetManeuverHolders()
                .Where(n =>
                    n.Bearing == ManeuverBearing.Turn
                    && n.Direction == ManeuverDirection.Right
                )
                .OrderBy(n => n.SpeedInt)
                .FirstOrDefault();
            bestTurnManeuvers.Add(bestTurnManeuver.ToString());

            return bestTurnManeuvers;
        }

        private static void ProcessMovementPredicition()
        {
            //Distance
            float minDistanceToEnenmyShip = float.MaxValue;
            foreach (GenericShip enemyShip in CurrentShip.Owner.EnemyShips.Values)
            {
                DistanceInfo distInfo = new DistanceInfo(CurrentShip, enemyShip);
                if (distInfo.MinDistance.DistanceReal < minDistanceToEnenmyShip) minDistanceToEnenmyShip = distInfo.MinDistance.DistanceReal;
            }

            //In arc - improve
            int enemiesInShotRange = 0;
            float minDistanceToNearestEnemyInShotRange = 0;
            foreach (GenericShip enemyShip in CurrentShip.Owner.EnemyShips.Values)
            {
                ShotInfo shotInfo = new ShotInfo(CurrentShip, enemyShip, CurrentShip.PrimaryWeapons.First());
                if (shotInfo.IsShotAvailable)
                {
                    enemiesInShotRange++;
                    if (minDistanceToNearestEnemyInShotRange < shotInfo.DistanceReal) minDistanceToNearestEnemyInShotRange = shotInfo.DistanceReal;
                }
            }

            NavigationResult result = new NavigationResult()
            {
                movementComplexity = CurrentMovementPrediction.CurrentMovement.ColorComplexity,
                distanceToNearestEnemy = minDistanceToEnenmyShip,
                distanceToNearestEnemyInShotRange = minDistanceToNearestEnemyInShotRange,
                enemiesInShotRange = enemiesInShotRange,
                isBumped = CurrentMovementPrediction.IsBumped,
                isLandedOnObstacle = CurrentMovementPrediction.IsLandedOnAsteroid,
                obstaclesHit = CurrentMovementPrediction.AsteroidsHit.Count,
                isOffTheBoard = CurrentMovementPrediction.IsOffTheBoard,
                minesHit = CurrentMovementPrediction.MinesHit.Count,
                isOffTheBoardNextTurn = !NextTurnNavigationResults.Any(n => !n.isOffTheBoard),
                isHitAsteroidNextTurn = !NextTurnNavigationResults.Any(n => n.obstaclesHit == 0)
            };
            result.CalculatePriority();

            NavigationResults.Add(
                CurrentMovementPrediction.CurrentMovement.ToString(),
                result
            );
        }

        private static void FinishManeuverPredicition()
        {
            VirtualBoard.RestoreBoard();

            Debug.Log("ALL RESULTS:");
            foreach (var result in NavigationResults)
            {
                Debug.Log(result.Key + ": " + result.Value.Priority);
            }

            BestManeuver = NavigationResults.OrderByDescending(n => n.Value.Priority).First().Key;
            Debug.Log("PREFERED RESULT: " + BestManeuver);
        }
    }
}

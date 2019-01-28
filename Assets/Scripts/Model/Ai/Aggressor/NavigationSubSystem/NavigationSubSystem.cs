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
            yield return PredictSimpleManeuversOfAllShips();
            yield return PredictManeuversOfThisShip();
            FinishManeuverPredicition();

            callback();
        }

        private static IEnumerator PredictSimpleManeuversOfAllShips()
        {
            List<GenericShip> shipsSorted = Roster.AllShips.Values
                .OrderByDescending(n => n.Owner.PlayerNo == Phases.PlayerWithInitiative)
                .OrderBy(n => n.State.Initiative)
                .ToList();

            foreach (GenericShip ship in shipsSorted)
            {
                yield return PredictSimpleManeuver(ship);
                VirtualBoard.SetVirtualPositionInfo(ship, CurrentMovementPrediction.FinalPositionInfo);
                if (IsActivationBeforeCurrentShip(ship)) VirtualBoard.SwitchToVirtualPosition(ship);
            }
        }

        private static IEnumerator PredictSimpleManeuver(GenericShip ship)
        {
            Selection.ChangeActiveShip(ship);

            GenericMovement savedMovement = ship.AssignedManeuver;

            GenericMovement movement = ShipMovementScript.MovementFromString("2.F.S");
            ship.SetAssignedManeuver(movement);

            CurrentMovementPrediction = new MovementPrediction(movement);
            yield return CurrentMovementPrediction.CalculateMovementPredicition();

            if (savedMovement != null)
            {
                ship.SetAssignedManeuver(savedMovement);
            }
            else
            {
                ship.ClearAssignedManeuver();
            }
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

                VirtualBoard.SetVirtualPositionInfo(CurrentShip, CurrentMovementPrediction.FinalPositionInfo);
                VirtualBoard.SwitchToVirtualPosition(CurrentShip);
                yield return CheckNextTurnRecursive(GetShortestTurnManeuvers());

                yield return ProcessMovementPredicition();

                VirtualBoard.SwitchToRealPosition(CurrentShip);
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

        private static IEnumerator ProcessMovementPredicition()
        {
            //Set positions of ships that move later

            List<GenericShip> shipsSorted = Roster.AllShips.Values
                .OrderByDescending(n => n.Owner.PlayerNo == Phases.PlayerWithInitiative)
                .OrderBy(n => n.State.Initiative)
                .Where(n => n != CurrentShip)
                .ToList();

            foreach (GenericShip ship in shipsSorted)
            {
                VirtualBoard.SwitchToVirtualPosition(ship);

                //Check possible collisions
                DistanceInfo distInfo = new DistanceInfo(CurrentShip, ship);
                if (distInfo.Range <= 1)
                {
                    //Re-check movement
                    yield return PredictSimpleManeuver(ship);
                }
            }

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

            //Restore positions of ships that move later
            foreach (GenericShip ship in shipsSorted.Where(n => !IsActivationBeforeCurrentShip(n)))
            {
                VirtualBoard.SwitchToRealPosition(ship);
            }
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

        private static bool IsActivationBeforeCurrentShip(GenericShip ship)
        {
            return ship.State.Initiative < CurrentShip.State.Initiative || (ship.State.Initiative == CurrentShip.State.Initiative && ship.Owner.PlayerNo == Phases.PlayerWithInitiative);
        }
    }
}

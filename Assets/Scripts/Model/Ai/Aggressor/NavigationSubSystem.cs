using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoardTools;
using Movement;
using Ship;
using UnityEngine;

namespace AI.Aggressor
{
    public class NavigationResult
    {
        public bool isOffTheBoard;
        public bool isLandedOnObstacle;

        public int enemiesInShotRange;

        public int obstaclesHit;
        public int minesHit;

        public float distanceToNearestEnemy;
        public float distanceToNearestEnemyInShotRange;

        public bool isOffTheBoardNextTurn;
        public bool isHitAsteroidNextTurn;

        public bool isBumped;
        
        public MovementComplexity movementComplexity;

        public int Priority { get; private set; }

        public void CalculatePriority()
        {
            if (isOffTheBoard) {
                Priority = int.MinValue;
                return;
            }

            if (isLandedOnObstacle) Priority -= 10000;

            if (isOffTheBoardNextTurn) Priority -= 20000;

            // TODO: Koigogran Turn ignores rotation
            Priority += enemiesInShotRange * 1000;

            Priority -= obstaclesHit * 2000;
            Priority -= minesHit * 2000;

            if (isHitAsteroidNextTurn) Priority -= 1000;

            if (isBumped) Priority -= 1000;

            switch (movementComplexity)
            {
                case MovementComplexity.Easy:
                    if (Selection.ThisShip.IsStressed) Priority += 500;
                    break;
                case MovementComplexity.Complex:
                    if (Selection.ThisShip.IsStressed)
                    {
                        Priority = int.MinValue;
                    }
                    else
                    {
                        Priority -= 500;
                    }
                    break;
                default:
                    break;
            }

            //0..10 * 10, prefers big distance
            Priority += ((int)distanceToNearestEnemy) * 10;
        }
    }

    public class ShipPosition
    {
        public Vector3 Position { get; private set; }
        public Vector3 Angles { get; private set; }

        public ShipPosition(Vector3 position, Vector3 angles)
        {
            Position = position;
            Angles = angles;
        }
    }

    public static class NavigationSubSystem
    {
        private static ShipPosition RealPosition;
        private static GenericShip CurrentShip;
        private static Action Callback;

        private static Dictionary<string, MovementComplexity> AllManeuvers;
        private static Dictionary<string, NavigationResult> NavigationResults;
        private static MovementPrediction CurrentMovementPrediction;

        private static List<NavigationResult> NextTurnNavigationResults;
        private static MovementPrediction CurrentTurnMovementPrediction;

        private static Dictionary<GenericShip, ShipPosition> SavedShipPositions;

        public static string BestManeuver { get; private set; }

        public static void CalculateNavigation(Action callback)
        {
            CurrentShip = Selection.ThisShip;
            Callback = callback;
            SavedShipPositions = new Dictionary<GenericShip, ShipPosition>();

            AllManeuvers = CurrentShip.GetManeuvers();
            NavigationResults = new Dictionary<string, NavigationResult>();

            PredictPositionsOfEnemyShipsRecursive();
        }

        private static void PredictPositionsOfEnemyShipsRecursive()
        {
            GenericShip enemyShip = CurrentShip.Owner.EnemyShips.Where(n => !SavedShipPositions.ContainsKey(n.Value)).FirstOrDefault().Value;

            if (enemyShip != null)
            {
                Selection.ChangeActiveShip(enemyShip);

                GenericMovement movement = ShipMovementScript.MovementFromString("2.F.S");
                enemyShip.SetAssignedManeuver(movement);

                CurrentMovementPrediction = new MovementPrediction(movement, FinishEnemyManeuverPredicition);
            }
            else
            {
                //TODO: Predict Maneuvers of Friendly Ships with lower Initiative
                PredictManeuversRecusive();
            }
        }

        private static void FinishEnemyManeuverPredicition()
        {
            SavedShipPositions.Add(
                Selection.ThisShip,
                new ShipPosition(
                    Selection.ThisShip.GetPosition(),
                    Selection.ThisShip.GetAngles()
                )
            );

            Selection.ThisShip.SetPosition(CurrentMovementPrediction.FinalPosition);
            Selection.ThisShip.SetAngles(CurrentMovementPrediction.FinalAngles);

            PredictPositionsOfEnemyShipsRecursive();
        }

        private static void PredictManeuversRecusive()
        {
            Selection.ChangeActiveShip(CurrentShip);

            if (AllManeuvers.Count > 0)
            {
                var firstManeuver = AllManeuvers.First();
                AllManeuvers.Remove(firstManeuver.Key);

                GenericMovement movement = ShipMovementScript.MovementFromString(firstManeuver.Key);
                CurrentShip.SetAssignedManeuver(movement);
                movement.Initialize();
                movement.IsSimple = true;
                CurrentMovementPrediction = new MovementPrediction(movement, StartCheckNextTurn);
            }
            else
            {
                FinishManeuverPredicition();
            }
        }

        private static void StartCheckNextTurn()
        {
            RealPosition = new ShipPosition(
                Selection.ThisShip.GetPosition(),
                Selection.ThisShip.GetAngles()
            );

            Selection.ThisShip.SetPosition(CurrentMovementPrediction.FinalPosition);
            Selection.ThisShip.SetAngles(CurrentMovementPrediction.FinalAngles);

            NextTurnNavigationResults = new List<NavigationResult>();
            CheckNextTurnRecursive(GetTurnManeuvers());
        }

        private static void CheckNextTurnRecursive(List<string> turnManeuvers)
        {
            if (turnManeuvers.Count == 0)
            {
                ProcessMovementPredicition();
            }
            else
            {
                string maneuverToTest = turnManeuvers.First();
                turnManeuvers.Remove(maneuverToTest);

                GenericMovement movement = ShipMovementScript.MovementFromString(maneuverToTest);
                CurrentShip.SetAssignedManeuver(movement);
                movement.Initialize();
                movement.IsSimple = true;
                CurrentTurnMovementPrediction = new MovementPrediction(movement, delegate { ProcessCheckNextTurnRecursive(turnManeuvers); });
            }
        }

        private static void ProcessCheckNextTurnRecursive(List<string> turnManeuvers)
        {
            NextTurnNavigationResults.Add(new NavigationResult()
            {
                isOffTheBoard = CurrentTurnMovementPrediction.IsOffTheBoard,
                obstaclesHit = CurrentMovementPrediction.AsteroidsHit.Count
            });

            CheckNextTurnRecursive(turnManeuvers);
        }

        private static List<string> GetTurnManeuvers()
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

            // Restore
            CurrentShip.SetPosition(RealPosition.Position);
            CurrentShip.SetAngles(RealPosition.Angles);

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

            PredictManeuversRecusive();
        }

        private static void FinishManeuverPredicition()
        {
            RestorePositionsOfEnemyShips();

            Debug.Log("ALL RESULTS:");
            foreach (var result in NavigationResults)
            {
                Debug.Log(result.Key + ": " + result.Value.Priority);
            }

            BestManeuver = NavigationResults.OrderByDescending(n => n.Value.Priority).First().Key;
            Debug.Log("PREFERED RESULT: " + BestManeuver);

            Callback();
        }

        private static void RestorePositionsOfEnemyShips()
        {
            foreach (var record in SavedShipPositions)
            {
                record.Key.SetPosition(record.Value.Position);
                record.Key.SetAngles(record.Value.Angles);
            }
        }
    }
}

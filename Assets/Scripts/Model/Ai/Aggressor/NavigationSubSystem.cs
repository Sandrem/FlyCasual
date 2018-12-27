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
        public float distanceToNearestEnemy;
    }

    public static class NavigationSubSystem
    {
        private static Action Callback;

        private static Dictionary<string, MovementComplexity> AllManeuvers;
        private static Dictionary<string, NavigationResult> NavigationResults;
        private static MovementPrediction CurrentMovementPrediction;

        public static string BestManeuver { get; private set; }

        public static void CalculateNavigation(Action callback)
        {
            Callback = callback;

            AllManeuvers = Selection.ThisShip.GetManeuvers();
            NavigationResults = new Dictionary<string, NavigationResult>();

            PredictManveuverRecusive();
        }

        private static void PredictManveuverRecusive()
        {
            if (AllManeuvers.Count > 0)
            {
                var firstManeuver = AllManeuvers.First();
                AllManeuvers.Remove(firstManeuver.Key);

                Debug.Log("Test: " + firstManeuver.Key);

                GenericMovement movement = ShipMovementScript.MovementFromString(firstManeuver.Key);
                Selection.ThisShip.SetAssignedManeuver(movement);
                movement.Initialize();
                CurrentMovementPrediction = new MovementPrediction(movement, ProcessMovementPredicition);
            }
            else
            {
                FinishManeuverPredicition();
            }
        }

        private static void ProcessMovementPredicition()
        {
            Vector3 realPosition = Selection.ThisShip.GetPosition();
            Vector3 realAngles = Selection.ThisShip.GetAngles();

            Selection.ThisShip.SetPosition(CurrentMovementPrediction.FinalPosition);
            Selection.ThisShip.SetAngles(CurrentMovementPrediction.FinalAngles);

            float minDistanceToEnenmyShip = float.MaxValue;
            foreach (GenericShip enemyShip in Selection.ThisShip.Owner.EnemyShips.Values)
            {
                DistanceInfo distInfo = new DistanceInfo(Selection.ThisShip, enemyShip);
                if (distInfo.MinDistance.DistanceReal < minDistanceToEnenmyShip) minDistanceToEnenmyShip = distInfo.MinDistance.DistanceReal;
            }

            Selection.ThisShip.SetPosition(realPosition);
            Selection.ThisShip.SetAngles(realAngles);

            NavigationResults.Add(
                CurrentMovementPrediction.CurrentMovement.ToString(),
                new NavigationResult()
                {
                    distanceToNearestEnemy = minDistanceToEnenmyShip
                }
            );

            PredictManveuverRecusive();
        }

        private static void FinishManeuverPredicition()
        {
            foreach (var result in NavigationResults)
            {
                Debug.Log(result.Key + " " + result.Value.distanceToNearestEnemy);
            }

            Debug.Log("---");

            BestManeuver = NavigationResults.OrderBy(n => n.Value.distanceToNearestEnemy).First().Key;
            Debug.Log(BestManeuver);

            Callback();
        }
    }
}

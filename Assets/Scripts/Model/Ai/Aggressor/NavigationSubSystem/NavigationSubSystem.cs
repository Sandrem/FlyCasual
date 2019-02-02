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

        private static Dictionary<string, NavigationResult> NavigationResults;
        private static MovementPrediction CurrentMovementPrediction;

        private static MovementPrediction CurrentSimpleMovementPrediction;

        private static List<NavigationResult> NextTurnNavigationResults;
        private static MovementPrediction CurrentTurnMovementPrediction;

        private static Dictionary<Players.PlayerNo, VirtualBoard> VirtualBoards;
        private static VirtualBoard VirtualBoard {
            get
            {
                if (!VirtualBoards.ContainsKey(CurrentShip.Owner.PlayerNo)) VirtualBoards.Add(CurrentShip.Owner.PlayerNo, null);
                return VirtualBoards[CurrentShip.Owner.PlayerNo];
            }
            set
            {
                if (!VirtualBoards.ContainsKey(CurrentShip.Owner.PlayerNo)) VirtualBoards.Add(CurrentShip.Owner.PlayerNo, null);
                VirtualBoards[CurrentShip.Owner.PlayerNo] = value;
            }
        }

        public static string BestManeuver { get; private set; }

        public static void Initialize()
        {
            VirtualBoards = new Dictionary<Players.PlayerNo, VirtualBoard>();
        }

        public static void CalculateNavigation(GenericShip ship, Action callback)
        {
            //Debug.Log("Start: " + ship);

            CurrentShip = ship;

            // TODO: for each player
            if (VirtualBoard == null)
            {
                VirtualBoard = new VirtualBoard();
            }
            else
            {
                VirtualBoard.Update();
            }

            GameManagerScript.Instance.StartCoroutine(StartCalculations(callback));
        }

        private static IEnumerator StartCalculations(Action callback)
        {
            Roster.ToggleStatusPanel(CurrentShip.Owner.PlayerNo, true);

            yield return PredictSimpleManeuversOfAllShips();
            yield return PredictManeuversOfThisShip();
            FinishManeuverPredicition();

            Roster.ToggleStatusPanel(CurrentShip.Owner.PlayerNo, false);

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
                //Generate virtual positions if they are not present
                if (!VirtualBoard.IsVirtualPositionReady(ship))
                {
                    yield return PredictSimpleManeuver(ship);
                    VirtualBoard.SetVirtualPositionInfo(ship, CurrentSimpleMovementPrediction.FinalPositionInfo);
                }

                if (IsActivationBeforeCurrentShip(ship)) VirtualBoard.SwitchToVirtualPosition(ship);
            }
        }

        private static IEnumerator PredictSimpleManeuver(GenericShip ship)
        {
            Selection.ThisShip = ship;

            GenericMovement savedMovement = ship.AssignedManeuver;

            GenericMovement movement = ShipMovementScript.MovementFromString( (ship.State.IsIonized) ? "1.F.S" : "2.F.S");
            ship.SetAssignedManeuver(movement, isSilent: true);
            movement.Initialize();
            movement.IsSimple = true;
            CurrentSimpleMovementPrediction = new MovementPrediction(movement);
            yield return CurrentSimpleMovementPrediction.CalculateMovementPredicition();

            if (savedMovement != null)
            {
                ship.SetAssignedManeuver(savedMovement, isSilent: true);
            }
            else
            {
                ship.ClearAssignedManeuver();
            }
        }

        private static IEnumerator PredictManeuversOfThisShip()
        {
            Selection.ChangeActiveShip(CurrentShip);

            NavigationResults = new Dictionary<string, NavigationResult>();

            foreach (var maneuver in CurrentShip.GetManeuvers())
            {
                GenericMovement movement = ShipMovementScript.MovementFromString(maneuver.Key);
                CurrentShip.SetAssignedManeuver(movement, isSilent: true);
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

            VirtualBoard.RemoveCollisionsExcept(CurrentShip);
            foreach (string turnManeuver in turnManeuvers)
            {
                GenericMovement movement = ShipMovementScript.MovementFromString(turnManeuver);
                CurrentShip.SetAssignedManeuver(movement, isSilent: true);
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
            VirtualBoard.ReturnCollisionsExcept(CurrentShip);
        }

        private static List<string> GetShortestTurnManeuvers()
        {
            List<string> bestTurnManeuvers = new List<string>();

            ManeuverHolder bestTurnManeuver = CurrentShip.GetManeuverHolders()
                .Where(n =>
                    n.Bearing == ManeuverBearing.Turn
                    && n.Direction == ManeuverDirection.Left
                )
                .OrderBy(n => n.SpeedInt)
                .FirstOrDefault();
            bestTurnManeuvers.Add(bestTurnManeuver.ToString());

            bestTurnManeuver = CurrentShip.GetManeuverHolders()
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
            //Save current virtual positions

            Dictionary<GenericShip, ShipPositionInfo> defaultVirtualPositions = new Dictionary<GenericShip, ShipPositionInfo>();

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
                if (!IsActivationBeforeCurrentShip(ship))
                {
                    DistanceInfo distInfo = new DistanceInfo(CurrentShip, ship);
                    if (distInfo.Range <= 1)
                    {
                        //Save old prediction and re-check movement
                        defaultVirtualPositions.Add(ship, VirtualBoard.Ships[ship].VirtualPositionInfo);
                        yield return PredictSimpleManeuver(ship);
                        VirtualBoard.SetVirtualPositionInfo(ship, CurrentSimpleMovementPrediction.FinalPositionInfo);
                        Selection.ChangeActiveShip(CurrentShip);
                    }
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
                movement = CurrentMovementPrediction.CurrentMovement,
                distanceToNearestEnemy = minDistanceToEnenmyShip,
                distanceToNearestEnemyInShotRange = minDistanceToNearestEnemyInShotRange,
                enemiesInShotRange = enemiesInShotRange,
                isBumped = CurrentMovementPrediction.IsBumped,
                isLandedOnObstacle = CurrentMovementPrediction.IsLandedOnAsteroid,
                obstaclesHit = CurrentMovementPrediction.AsteroidsHit.Count,
                isOffTheBoard = CurrentMovementPrediction.IsOffTheBoard,
                minesHit = CurrentMovementPrediction.MinesHit.Count,
                isOffTheBoardNextTurn = !NextTurnNavigationResults.Any(n => !n.isOffTheBoard),
                isHitAsteroidNextTurn = !NextTurnNavigationResults.Any(n => n.obstaclesHit == 0),
                FinalPositionInfo = CurrentMovementPrediction.FinalPositionInfo
            };
            result.CalculatePriority();

            NavigationResults.Add(
                CurrentMovementPrediction.CurrentMovement.ToString(),
                result
            );

            //Restore previous virtual positions
            foreach (var shipInfo in defaultVirtualPositions)
            {
                VirtualBoard.SetVirtualPositionInfo(shipInfo.Key, shipInfo.Value);
            }

            //Restore positions of ships that move later
            foreach (GenericShip ship in shipsSorted.Where(n => !IsActivationBeforeCurrentShip(n)))
            {
                VirtualBoard.SwitchToRealPosition(ship);
            }
        }

        private static void FinishManeuverPredicition()
        {
            VirtualBoard.RestoreBoard();

            //Debug.Log("ALL RESULTS:");
            foreach (var result in NavigationResults)
            {
                //Debug.Log(result.Key + ": " + result.Value.Priority);
            }

            int bestNavigationIdePriority = NavigationResults.Values.Max(n => n.Priority);
            var bestNavigationIdeas = NavigationResults.Where(n => n.Value.Priority == bestNavigationIdePriority).ToDictionary(n => n.Key, m => m.Value);

            if (bestNavigationIdeas.Any(n => n.Value.movement.Direction == ManeuverDirection.Forward))
            {
                BestManeuver = bestNavigationIdeas.FirstOrDefault(n => n.Value.movement.Direction == ManeuverDirection.Forward).Key;
            }
            else
            {
                BestManeuver = bestNavigationIdeas.First().Key;
            }

            VirtualBoard.SetVirtualPositionInfo(CurrentShip, bestNavigationIdeas[BestManeuver].FinalPositionInfo);
            //Debug.Log("PREFERED RESULT: " + BestManeuver);
        }

        private static bool IsActivationBeforeCurrentShip(GenericShip ship)
        {
            return ship.State.Initiative < CurrentShip.State.Initiative
                || (ship.State.Initiative == CurrentShip.State.Initiative && ship.Owner.PlayerNo == Phases.PlayerWithInitiative && ship.Owner.PlayerNo != CurrentShip.Owner.PlayerNo)
                || (ship.State.Initiative == CurrentShip.State.Initiative && ship.ShipId < CurrentShip.ShipId && ship.Owner.PlayerNo == CurrentShip.Owner.PlayerNo);
        }
    }
}

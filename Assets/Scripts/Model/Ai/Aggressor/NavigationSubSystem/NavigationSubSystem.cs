﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BoardTools;
using Movement;
using Players;
using Ship;
using UnityEngine;

/*
 *  TODO:
 *
 *  Angle to enemy ship
 *  Left/Right turn checks
 * 
 */

namespace AI.Aggressor
{
    public static class NavigationSubSystem
    {
        private static GenericPlayer CurrentPlayer;

        private static Dictionary<PlayerNo, VirtualBoard> VirtualBoards;
        private static VirtualBoard VirtualBoard
        {
            get { return VirtualBoards[CurrentPlayer.PlayerNo]; }
            set { VirtualBoards[CurrentPlayer.PlayerNo] = value; }
        }

        private static int OrderOfActivation;

        public static void CalculateNavigation(Action callback)
        {
            CurrentPlayer = Roster.GetPlayer(Phases.CurrentSubPhase.RequiredPlayer);

            ConfigureVirtualBoards();

            if (DebugManager.DebugAiNavigation)
            {
                Console.Write("Navigation calculations are started", LogTypes.AI, isBold: true);
            }

            GameManagerScript.Instance.StartCoroutine
            (
                StartCalculations(callback)
            );
        }

        private static IEnumerator StartCalculations(Action callback)
        {
            ShowCalculationsStart();

            SwitchEnemyShipsToSimpleVirtualPositions();
            yield return PredictAllFinalPositionsOfOwnShips();

            RestoreRealBoard();

            List<GenericShip> orderOfActivation = GenerateOrderOfActivation();

            yield return FindBestManeuversForShips(orderOfActivation);

            RestoreRealBoard();
            ShowCalculationsEnd();

            callback();
        }

        private static void SwitchEnemyShipsToSimpleVirtualPositions()
        {
            foreach (GenericShip ship in CurrentPlayer.EnemyShips.Values)
            {
                PredictSimpleFinalPositionOfEnemyShip(ship);
            }
        }

        private static void PredictSimpleFinalPositionOfEnemyShip(GenericShip ship)
        {
            Selection.ThisShip = ship;

            GenericMovement savedMovement = ship.AssignedManeuver;

            // Decide what maneuvers to use as temporary
            string temporyManeuver = (ship.State.IsIonized) ? "1.F.S" : "2.F.S";
            bool isTemporaryManeuverAdded = false;
            if (!ship.HasManeuver(temporyManeuver))
            {
                isTemporaryManeuverAdded = true;
                ship.Maneuvers.Add(temporyManeuver, MovementComplexity.Easy);
            }
            GenericMovement movement = ShipMovementScript.MovementFromString(temporyManeuver);

            // Check maneuver
            ship.SetAssignedManeuver(movement, isSilent: true);
            movement.Initialize();
            movement.IsSimple = true;

            MovementPrediction prediction = new MovementPrediction(movement);
            prediction.GenerateFinalShipStand();
            prediction.CalculateOnlyFinalPosition();

            if (isTemporaryManeuverAdded)
            {
                ship.Maneuvers.Remove(temporyManeuver);
            }

            if (savedMovement != null)
            {
                ship.SetAssignedManeuver(savedMovement, isSilent: true);
            }
            else
            {
                ship.ClearAssignedManeuver();
            }

            VirtualBoard.SetVirtualPositionInfo(ship, prediction.FinalPositionInfo, temporyManeuver);
        }

        private static IEnumerator PredictAllFinalPositionsOfOwnShips()
        {
            foreach (GenericShip ship in CurrentPlayer.EnemyShips.Values)
            {
                VirtualBoard.SwitchToVirtualPosition(ship);
            }

            foreach (GenericShip ship in CurrentPlayer.Ships.Values)
            {
                yield return PredictFinalPosionsOfOwnShip(ship);
            }

            foreach (GenericShip ship in CurrentPlayer.EnemyShips.Values)
            {
                VirtualBoard.SwitchToRealPosition(ship);
            }
        }

        private static IEnumerator PredictFinalPosionsOfOwnShip(GenericShip ship)
        {
            Selection.ChangeActiveShip(ship);
            VirtualBoard.SwitchToRealPosition(ship);

            Dictionary<string, NavigationResult> navigationResults = new Dictionary<string, NavigationResult>();
            foreach (var maneuver in ship.GetManeuvers())
            {
                GenericMovement movement = ShipMovementScript.MovementFromString(maneuver.Key);
                ship.SetAssignedManeuver(movement, isSilent: true);
                movement.Initialize();
                movement.IsSimple = true;

                MovementPrediction prediction = new MovementPrediction(movement);
                prediction.GenerateFinalShipStand();
                prediction.CalculateOnlyFinalPosition();

                VirtualBoard.SetVirtualPositionInfo(ship, prediction.FinalPositionInfo, prediction.CurrentMovement.ToString());
                VirtualBoard.SwitchToVirtualPosition(ship);

                NavigationResult result = new NavigationResult()
                {
                    movement = prediction.CurrentMovement,
                    distanceToNearestEnemy = GetMinDistanceToEnemyShip(ship),
                    distanceToNearestEnemyInShotRange = GetMinDistanceToEnemyShipInShotRange(ship),
                    angleToNearestEnemy = GetAngleToNearestEnemy(ship),
                    enemiesInShotRange = GetEnemiesInShotRange(ship),
                    isBumped = prediction.IsBumped,
                    isLandedOnObstacle = prediction.IsLandedOnAsteroid,
                    isOffTheBoard = prediction.IsOffTheBoard,
                    FinalPositionInfo = prediction.FinalPositionInfo
                };
                result.CalculatePriority();

                if (DebugManager.DebugAiNavigation)
                {
                    Console.Write("", LogTypes.AI);
                }

                navigationResults.Add(maneuver.Key, result);

                VirtualBoard.SwitchToRealPosition(ship);

                yield return true;
            }

            ship.ClearAssignedManeuver();
            VirtualBoard.UpdateNavigationResults(ship, navigationResults);
        }

        private static float GetMinDistanceToEnemyShip(GenericShip ship)
        {
            float minDistanceToEnemyShip = float.MaxValue;

            foreach (GenericShip enemyShip in ship.Owner.EnemyShips.Values)
            {
                DistanceInfo distInfo = new DistanceInfo(ship, enemyShip);
                if (distInfo.MinDistance.DistanceReal < minDistanceToEnemyShip)
                {
                    minDistanceToEnemyShip = distInfo.MinDistance.DistanceReal;
                }
            }

            return minDistanceToEnemyShip;
        }

        private static float GetMinDistanceToEnemyShipInShotRange(GenericShip ship)
        {
            float minDistanceToNearestEnemyInShotRange = 0;

            foreach (GenericShip enemyShip in ship.Owner.EnemyShips.Values)
            {
                ShotInfo shotInfo = new ShotInfo(ship, enemyShip, ship.PrimaryWeapons.First());
                if (shotInfo.IsShotAvailable)
                {
                    if (minDistanceToNearestEnemyInShotRange < shotInfo.DistanceReal)
                    {
                        minDistanceToNearestEnemyInShotRange = shotInfo.DistanceReal;
                    }
                }
            }

            return minDistanceToNearestEnemyInShotRange;
        }

        private static float GetAngleToNearestEnemy(GenericShip ship)
        {
            // TODO: Needs to rotate ship if enemy is on tail

            return 0;
        }

        private static int GetEnemiesInShotRange(GenericShip ship)
        {
            int enemiesInShotRange = 0;

            foreach (GenericShip enemyShip in ship.Owner.EnemyShips.Values)
            {
                ShotInfo shotInfo = new ShotInfo(ship, enemyShip, ship.PrimaryWeapons.First());
                if (shotInfo.IsShotAvailable)
                {
                    enemiesInShotRange++;
                }
            }

            return enemiesInShotRange;
        }

        private static List<GenericShip> GenerateOrderOfActivation()
        {
            OrderOfActivation = 0;
            
            List<GenericShip> orderOfActivation = new List<GenericShip>();

            List<GenericShip> AllShips = new List<GenericShip>(Roster.AllShips.Values.ToList());

            while (AllShips.Count > 0)
            {
                int lowestInitiative = AllShips.Min(n => n.State.Initiative);

                GenericShip shipToActivate = AllShips
                    .Where(n => n.State.Initiative == lowestInitiative)
                    .OrderBy(n => GetMinDistanceToEnemyShip(n))
                    .OrderByDescending(n => n.Owner.PlayerNo == Phases.PlayerWithInitiative)
                    .First();

                orderOfActivation.Add(shipToActivate);
                AllShips.Remove(shipToActivate);
            }

            if (DebugManager.DebugAiNavigation)
            {
                Console.Write("", LogTypes.AI);
                Console.Write("Order of activation is predicted:", LogTypes.AI, isBold: true);

                string orderOfActivationText = "";
                foreach (GenericShip ship in orderOfActivation)
                {
                    orderOfActivationText += (ship.ShipId + ", ");
                }
                Console.Write(orderOfActivationText, LogTypes.AI);
            }

            return orderOfActivation;
        }

        private static IEnumerator FindBestManeuversForShips(List<GenericShip> orderOfActivation)
        {
            while (orderOfActivation.Count > 0)
            {
                SetVirtualPositionsForShipsWithPreviousActivations(orderOfActivation);

                GenericShip ship = orderOfActivation.First();
                orderOfActivation.Remove(ship);

                if (ship.Owner.PlayerNo == CurrentPlayer.PlayerNo)
                {
                    yield return FindBestManeuver(ship);
                }
                else
                {
                    yield return PredictCollisionDetectionOfEnemyShip(ship);
                }
            }
        }

        private static IEnumerator FindBestManeuver(GenericShip ship)
        {
            Selection.ChangeActiveShip(ship);

            if (DebugManager.DebugAiNavigation)
            {
                Console.Write("", LogTypes.AI);
                Console.Write("Best maneuver calculations for " + ship.ShipId, LogTypes.AI, isBold: true);
            }

            int bestPriority = int.MinValue;
            KeyValuePair<string, NavigationResult> maneuverToCheck = new KeyValuePair<string, NavigationResult>();

            do
            {
                VirtualBoard.SwitchToRealPosition(ship);

                bestPriority = VirtualBoard.Ships[ship].NavigationResults.Max(n => n.Value.Priority);
                maneuverToCheck = VirtualBoard.Ships[ship].NavigationResults.Where(n => n.Value.Priority == bestPriority).First();

                if (DebugManager.DebugAiNavigation)
                {
                    Console.Write("Current best maneuver is " + maneuverToCheck.Key + " with priority " + maneuverToCheck.Value.ToString(), LogTypes.AI);
                }

                GenericMovement movement = ShipMovementScript.MovementFromString(maneuverToCheck.Key);

                ship.SetAssignedManeuver(movement, isSilent: true);
                movement.Initialize();
                movement.IsSimple = true;

                MovementPrediction prediction = new MovementPrediction(movement);
                prediction.GenerateShipStands();
                yield return prediction.CalculateMovementPredicition();

                VirtualBoard.SetVirtualPositionInfo(ship, prediction.FinalPositionInfo, prediction.CurrentMovement.ToString());
                VirtualBoard.SwitchToVirtualPosition(ship);

                NavigationResult result = new NavigationResult()
                {
                    movement = prediction.CurrentMovement,
                    isBumped = prediction.IsBumped,
                    isLandedOnObstacle = prediction.IsLandedOnAsteroid,
                    obstaclesHit = prediction.AsteroidsHit.Count,
                    isOffTheBoard = prediction.IsOffTheBoard,
                    minesHit = prediction.MinesHit.Count,
                    isOffTheBoardNextTurn = false, //!NextTurnNavigationResults.Any(n => !n.isOffTheBoard),
                    isHitAsteroidNextTurn = false, //!NextTurnNavigationResults.Any(n => n.obstaclesHit == 0),
                    FinalPositionInfo = prediction.FinalPositionInfo
                };

                foreach (GenericShip enemyShip in CurrentPlayer.EnemyShips.Values)
                {
                    VirtualBoard.SwitchToVirtualPosition(enemyShip);
                }

                result.distanceToNearestEnemy = GetMinDistanceToEnemyShip(ship);
                result.distanceToNearestEnemyInShotRange = GetMinDistanceToEnemyShipInShotRange(ship);
                result.angleToNearestEnemy = GetAngleToNearestEnemy(ship);
                result.enemiesInShotRange = GetEnemiesInShotRange(ship);

                result.CalculatePriority();

                if (DebugManager.DebugAiNavigation)
                {
                    Console.Write("After reevaluation priority is changed to " + result.ToString(), LogTypes.AI);
                }

                VirtualBoard.Ships[ship].NavigationResults[maneuverToCheck.Key] = result;

                bestPriority = VirtualBoard.Ships[ship].NavigationResults.Max(n => n.Value.Priority);
                if (DebugManager.DebugAiNavigation)
                {
                    Console.Write("Highest priority of all maneuvers is " + bestPriority, LogTypes.AI);
                }

                VirtualBoard.SwitchToRealPosition(ship);

                maneuverToCheck = VirtualBoard.Ships[ship].NavigationResults.First(n => n.Key == maneuverToCheck.Key);

                foreach (GenericShip enemyShip in CurrentPlayer.EnemyShips.Values)
                {
                    VirtualBoard.SwitchToRealPosition(enemyShip);
                }

            } while (maneuverToCheck.Value.Priority != bestPriority);

            if (DebugManager.DebugAiNavigation)
            {
                Console.Write("Maneuver is chosen: " + maneuverToCheck.Key, LogTypes.AI);
            }

            VirtualBoard.Ships[ship].SetPlannedManeuverCode(maneuverToCheck.Key, ++OrderOfActivation);
            ship.ClearAssignedManeuver();
        }

        private static void SetVirtualPositionsForShipsWithPreviousActivations(List<GenericShip> orderOfActivation)
        {
            foreach (GenericShip ship in Roster.AllShips.Values)
            {
                if (!orderOfActivation.Contains(ship))
                {
                    VirtualBoard.SwitchToVirtualPosition(ship);
                }
            }
        }

        private static IEnumerator PredictCollisionDetectionOfEnemyShip(GenericShip ship)
        {
            Selection.ThisShip = ship;

            GenericMovement savedMovement = ship.AssignedManeuver;

            // Decide what maneuvers to use as temporary
            string temporyManeuver = (ship.State.IsIonized) ? "1.F.S" : "2.F.S";
            bool isTemporaryManeuverAdded = false;
            if (!ship.HasManeuver(temporyManeuver))
            {
                isTemporaryManeuverAdded = true;
                ship.Maneuvers.Add(temporyManeuver, MovementComplexity.Easy);
            }
            GenericMovement movement = ShipMovementScript.MovementFromString(temporyManeuver);

            // Check maneuver
            ship.SetAssignedManeuver(movement, isSilent: true);
            movement.Initialize();
            movement.IsSimple = true;

            MovementPrediction prediction = new MovementPrediction(movement);
            prediction.GenerateShipStands();
            yield return prediction.CalculateMovementPredicition();

            if (isTemporaryManeuverAdded)
            {
                ship.Maneuvers.Remove(temporyManeuver);
            }

            if (savedMovement != null)
            {
                ship.SetAssignedManeuver(savedMovement, isSilent: true);
            }
            else
            {
                ship.ClearAssignedManeuver();
            }

            VirtualBoard.SetVirtualPositionInfo(ship, prediction.FinalPositionInfo, temporyManeuver);
        }

        /*private static IEnumerator CheckNextTurnRecursive(List<string> turnManeuvers)
        {
            NextTurnNavigationResults = new List<NavigationResult>();

            VirtualBoard.RemoveCollisionsExcept(CurrentShip);
            foreach (string turnManeuver in turnManeuvers)
            {
                GenericMovement movement = ShipMovementScript.MovementFromString(turnManeuver);
                if (movement.Bearing == ManeuverBearing.Stationary) continue;

                CurrentShip.SetAssignedManeuver(movement, isSilent: true);
                movement.Initialize();
                movement.IsSimple = true;

                CurrentTurnMovementPrediction = new MovementPrediction(movement);
                CurrentTurnMovementPrediction.GenerateShipStands();
                yield return CurrentTurnMovementPrediction.CalculateMovementPredicition();

                NextTurnNavigationResults.Add(new NavigationResult()
                {
                    isOffTheBoard = CurrentTurnMovementPrediction.IsOffTheBoard,
                    obstaclesHit = CurrentTurnMovementPrediction.AsteroidsHit.Count
                });
            }
            VirtualBoard.ReturnCollisionsExcept(CurrentShip);
        }*/

        /*private static List<string> GetShortestTurnManeuvers()
        {
            List<string> bestTurnManeuvers = new List<string>();

            ManeuverHolder bestTurnManeuver = CurrentShip.GetManeuverHolders()
                .Where(n =>
                    n.Bearing == ManeuverBearing.Turn
                    && n.Direction == ManeuverDirection.Left
                )
                .OrderBy(n => n.SpeedIntUnsigned)
                .FirstOrDefault();
            bestTurnManeuvers.Add(bestTurnManeuver.ToString());

            bestTurnManeuver = CurrentShip.GetManeuverHolders()
                .Where(n =>
                    n.Bearing == ManeuverBearing.Turn
                    && n.Direction == ManeuverDirection.Right
                )
                .OrderBy(n => n.SpeedIntUnsigned)
                .FirstOrDefault();
            bestTurnManeuvers.Add(bestTurnManeuver.ToString());

            return bestTurnManeuvers;
        }*/

        public static GenericShip GetNextShipWithoutAssignedManeuver()
        {
            return Roster.GetPlayer(Phases.CurrentSubPhase.RequiredPlayer).Ships.Values
                .Where(n => n.AssignedManeuver == null && !n.State.IsIonized)
                .OrderBy(n => VirtualBoard.Ships[n].OrderToActivate)
                .FirstOrDefault();
        }

        public static GenericShip GetNextShipWithoutFinishedManeuver()
        {
            return Roster.GetPlayer(Phases.CurrentSubPhase.RequiredPlayer).Ships.Values
                .Where(n => !n.IsManeuverPerformed)
                .OrderBy(n => VirtualBoard.Ships[n].OrderToActivate)
                .FirstOrDefault();
        }

        public static void AssignPlannedManeuver(GenericShip ship, Action callBack)
        {
            ShipMovementScript.SendAssignManeuverCommand(ship.ShipId, VirtualBoard.Ships[ship].PlannedManeuverCode);
            callBack();
        }

        // Low Priority

        private static void ConfigureVirtualBoards()
        {
            if (Phases.RoundCounter == 1) VirtualBoards = new Dictionary<PlayerNo, VirtualBoard>()
            {
                { PlayerNo.Player1, new VirtualBoard() },
                { PlayerNo.Player2, new VirtualBoard() }
            };

            VirtualBoard.Update();
        }

        private static void ShowCalculationsStart()
        {
            Roster.ToggleCalculatingStatus(Phases.CurrentSubPhase.RequiredPlayer, true);
        }

        private static void ShowCalculationsEnd()
        {
            Roster.ToggleCalculatingStatus(Phases.CurrentSubPhase.RequiredPlayer, false);
        }

        private static void RestoreRealBoard()
        {
            VirtualBoard.RestoreBoard();
        }
    }
}

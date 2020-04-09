using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BoardTools;
using Movement;
using Players;
using Ship;
using UnityEngine;

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

        public static void CalculateNavigation(Action callback)
        {
            CurrentPlayer = Roster.GetPlayer(Phases.CurrentSubPhase.RequiredPlayer);

            ConfigureVirtualBoards();

            GameManagerScript.Instance.StartCoroutine
            (
                StartCalculations(callback)
            );
        }

        private static IEnumerator StartCalculations(Action callback)
        {
            ShowCalculationsStart();

            SwitchEnemyShipsToSimpleVirtualPositions();
            Debug.Log(Time.realtimeSinceStartup);
            yield return PredictAllFinalPositionsOfOwnShips();
            Debug.Log(Time.realtimeSinceStartup);

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
            List<GenericShip> orderOfActivation = new List<GenericShip>();

            List<GenericShip> AllShips = new List<GenericShip>(Roster.AllShips.Values.ToList());

            while (AllShips.Count > 0)
            {
                int lowestInitiative = AllShips.Min(n => n.State.Initiative);

                //TODO: Player Initiative
                //TODO: Distance to nearest enemy
                GenericShip shipToActivate = AllShips
                    .Where(n => n.State.Initiative == lowestInitiative)
                    .First();

                orderOfActivation.Add(shipToActivate);
                AllShips.Remove(shipToActivate);
            }

            return orderOfActivation;
        }

        private static IEnumerator FindBestManeuversForShips(List<GenericShip> orderOfActivation)
        {
            while (orderOfActivation.Count > 0)
            {
                GenericShip ship = orderOfActivation.First();
                orderOfActivation.Remove(ship);

                if (ship.Owner.PlayerNo == CurrentPlayer.PlayerNo)
                {
                    SetVirtualPositionsForShipsWithPreviousActivations(orderOfActivation);
                    yield return FindBestManeuver(ship);
                }
                else
                {
                    yield return UpdateManeuverOfEnemyShip();
                }
            }
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

        private static IEnumerator FindBestManeuver(GenericShip ship)
        {
            // TODO: Do all here
            VirtualBoard.Ships[ship].SetPlannedManeuverCode("2.F.S");
            yield return true;
        }

        private static IEnumerator UpdateManeuverOfEnemyShip()
        {
            // Just check maneuver collisions
            yield return true;
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

        /*private static IEnumerator ProcessMovementPredicition()
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

            yield return true;


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
                isOffTheBoardNextTurn = false, //!NextTurnNavigationResults.Any(n => !n.isOffTheBoard),
                isHitAsteroidNextTurn = false, //!NextTurnNavigationResults.Any(n => n.obstaclesHit == 0),
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
        }*/

        /*private static void FinishManeuverPredicition()
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

            BestManeuver = "2.F.S";
        }*/

        public static GenericShip GetNextShipWithoutAssignedManeuver()
        {
            return Roster.GetPlayer(Phases.CurrentSubPhase.RequiredPlayer).Ships.Values
                .Where(n => n.AssignedManeuver == null && !n.State.IsIonized)
                .OrderBy(n => VirtualBoard.Ships[n].ManeuverCodeAssignedTime)
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

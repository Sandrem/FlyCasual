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
        private static MovementPrediction CurrentMovementPrediction;

        private static MovementPrediction CurrentSimpleMovementPrediction;

        private static List<NavigationResult> NextTurnNavigationResults;
        private static MovementPrediction CurrentTurnMovementPrediction;

        private static Dictionary<PlayerNo, VirtualBoard> VirtualBoards;
        private static VirtualBoard VirtualBoard
        {
            get { return VirtualBoards[Phases.CurrentSubPhase.RequiredPlayer]; }
            set { VirtualBoards[Phases.CurrentSubPhase.RequiredPlayer] = value; }
        }

        public static string BestManeuver { get; private set; }

        public static void CalculateNavigation(Action callback)
        {
            if (Phases.RoundCounter == 1) VirtualBoards = new Dictionary<PlayerNo, VirtualBoard>()
            {
                { PlayerNo.Player1, new VirtualBoard() },
                { PlayerNo.Player2, new VirtualBoard() }
            };

            VirtualBoard.Update();

            GameManagerScript.Instance.StartCoroutine(StartCalculations(callback));
        }

        private static IEnumerator StartCalculations(Action callback)
        {
            Roster.ToggleCalculatingStatus(Phases.CurrentSubPhase.RequiredPlayer, true);

            PredictFinalPositionsOfShips();

            yield return AssignBestManeuversToOwnShips();

            Roster.ToggleCalculatingStatus(Phases.CurrentSubPhase.RequiredPlayer, false);

            callback();
        }

        private static void PredictFinalPositionsOfShips()
        {
            int save = 0;

            GenericShip ship = null;
            do
            {
                ship = GetNextShipForFinalPositionPrediction();

                if (ship != null)
                {
                    //Debug.Log("PredictFinalPositionsOfShips:" + ship.ShipId);
                    if (ship.Owner.PlayerNo != Phases.CurrentSubPhase.RequiredPlayer)
                    {
                        PredictSimpleFinalPositionOfEnemyShip(ship);
                    }
                    else
                    {
                        PredictFinalPosionsOfOwnShip(ship);
                    }
                }

                save++;

            } while (ship != null || save > 16);
        }

        private static GenericShip GetNextShipForFinalPositionPrediction()
        {
            return Roster.AllShips.Values
               .OrderBy(n => Board.DistanceToNearestEnemy(n))
               .OrderBy(n => n.State.Initiative)
               .FirstOrDefault(n => VirtualBoard.RequiresFinalPositionPrediction(n));
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

            CurrentSimpleMovementPrediction = new MovementPrediction(movement);
            CurrentSimpleMovementPrediction.GenerateFinalShipStand();
            CurrentSimpleMovementPrediction.CalculateOnlyFinalPosition();

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

            // Set as virtual position
            VirtualBoard.SetVirtualPositionInfo(ship, CurrentSimpleMovementPrediction.FinalPositionInfo, temporyManeuver);
            // VirtualBoard.SwitchToVirtualPosition(ship);
        }

        private static void PredictFinalPosionsOfOwnShip(GenericShip ship)
        {
            Selection.ChangeActiveShip(ship);

            Dictionary<string, NavigationResult> navigationResults = new Dictionary<string, NavigationResult>();
            foreach (var maneuver in ship.GetManeuvers())
            {
                GenericMovement movement = ShipMovementScript.MovementFromString(maneuver.Key);
                ship.SetAssignedManeuver(movement, isSilent: true);
                movement.Initialize();
                movement.IsSimple = true;

                CurrentMovementPrediction = new MovementPrediction(movement);
                CurrentMovementPrediction.GenerateFinalShipStand();
                CurrentMovementPrediction.CalculateOnlyFinalPosition();

                //VirtualBoard.SetVirtualPositionInfo(ship, CurrentMovementPrediction.FinalPositionInfo);
                //VirtualBoard.SwitchToVirtualPosition(ship);
                //yield return CheckNextTurnRecursive(GetShortestTurnManeuvers());

                // yield return ProcessMovementPredicition();

                // VirtualBoard.SwitchToRealPosition(ship);

                NavigationResult result = new NavigationResult()
                {
                    movement = CurrentMovementPrediction.CurrentMovement,
                    FinalPositionInfo = CurrentMovementPrediction.FinalPositionInfo
                };
                navigationResults.Add(maneuver.Key, result);
            }

            ship.ClearAssignedManeuver();
            VirtualBoard.Ships[ship].UpdateNavigationResults(navigationResults);
        }

        private static IEnumerator AssignBestManeuversToOwnShips()
        {
            int save = 0;

            GenericShip ship = null;
            do
            {
                ship = GetNextShipForManeuverAssignment();

                if (ship != null)
                {
                    //Debug.Log("AssignBestManeuversToOwnShips:" + ship.ShipId);
                    if (ship.Owner.PlayerNo == Phases.CurrentSubPhase.RequiredPlayer)
                    {
                        VirtualBoard.Ships[ship].SetManeuverCode("2.F.S");
                    }
                }

                save++;
            } while (ship != null || save > 16);

            yield return true;
        }

        private static GenericShip GetNextShipForManeuverAssignment()
        {
            return Roster.AllShips.Values
               .Where(n => n.Owner.PlayerNo == Phases.CurrentSubPhase.RequiredPlayer)
               .OrderBy(n => Board.DistanceToNearestEnemy(n))
               .OrderBy(n => n.State.Initiative)
               .FirstOrDefault(n => VirtualBoard.RequiresManeuverAssignment(n));
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
            ShipMovementScript.SendAssignManeuverCommand(ship.ShipId, VirtualBoard.Ships[ship].ManeuverCode);
            callBack();
        }
    }
}

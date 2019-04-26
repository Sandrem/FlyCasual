using ActionsList;
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

            string temporyManeuver = (ship.State.IsIonized) ? "1.F.S" : "2.F.S";
            bool isTemporaryManeuverAdded = false;
            if (!ship.HasManeuver(temporyManeuver))
            {
                isTemporaryManeuverAdded = true;
                ship.Maneuvers.Add(temporyManeuver, MovementComplexity.Easy);
            }
            GenericMovement movement = ShipMovementScript.MovementFromString(temporyManeuver);

            ship.SetAssignedManeuver(movement, isSilent: true);
            movement.Initialize();
            movement.IsSimple = true;
            CurrentSimpleMovementPrediction = new MovementPrediction(movement);
            yield return CurrentSimpleMovementPrediction.CalculateMovementPredicition();
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
                if (movement.Bearing == ManeuverBearing.Stationary) continue;

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

        // This function will try all moves related to a boost, barrelroll, decloak, SLAM, or tractor.  If the action is from Supernatural Reflexes or Advanced Sensors,
        // it will also test the maneuver that follows it to see if that is better or worse than without taking the action before it.
        public static int TryActionPossibilities(GenericMovement FinalSuggestion, GenericAction actionToTry, bool isBeforeManeuverPhase = false)
        {
            int result = 0;
            GenericShip thisShip = Selection.ActiveShip;
            GenericMovement savedMovement = thisShip.AssignedManeuver;
            FinalSuggestion = thisShip.AssignedManeuver;
            int shieldHullTotal = thisShip.State.ShieldsCurrent + thisShip.State.HullCurrent;
            int startingResult = 0;

            NavigationResult StartingPosition = null;
            ShipPositionInfo shipOriginalPosition = thisShip.GetPositionInfo();

            // Prepare our virtual board maneuver tests.
            VirtualBoard.SetVirtualPositionInfo(thisShip, shipOriginalPosition);
            VirtualBoard.SwitchToVirtualPosition(thisShip);



            // Set up our virtual board.
            if (VirtualBoard == null)
            {
                VirtualBoard = new VirtualBoard();
            }
            else
            {
                VirtualBoard.Update();
            }

            // Record our current position for comparison.
            if (isBeforeManeuverPhase == true)
            {
                // Our action is before a maneuver.  Find out the results for our maneuver if we don't boost/barrel-roll/decloak before it.
                MovementPrediction maneuverWithoutActionFirst = new MovementPrediction(thisShip.AssignedManeuver);
                VirtualBoard.SetVirtualPositionInfo(thisShip, maneuverWithoutActionFirst.FinalPositionInfo);
                StartingPosition = GetCurrentPositionNavigationInfo(thisShip);

                // Move us back to before the maneuver.
                VirtualBoard.SetVirtualPositionInfo(thisShip, shipOriginalPosition);
            }
            else
            {
                // Just record our current position.
                StartingPosition = GetCurrentPositionNavigationInfo(thisShip);
            }
            // Determine how good our starting position is.
            startingResult = CalculatePositionPriority(StartingPosition);

            // Test for a boost action.
            if (actionToTry is BoostAction)
            {

                int bestBoostResult = 0;
                GenericMovement bestBoostMove = null;
                NavigationResult bestBoostNavigation = null;

                NavigationResult currentBoostNavigation = null;
                int currentBoostResult = 0;

                // We're performing a boost action.  Check all boost action possibilities.
                List<BoostMove> AvailableBoostMoves = new List<BoostMove>();
                AvailableBoostMoves = thisShip.GetAvailableBoostTemplates();
                foreach(BoostMove move in AvailableBoostMoves)
                {
                    string selectedBoostHelper = move.Name;

                    MovementPrediction boostPrediction = null;
                    GenericMovement boostMovement;
                    // Use the name of our boost action to generate a GenericMovement of the matching type.
                    switch (selectedBoostHelper)
                    {
                        case "Straight 1":
                            boostMovement = new StraightBoost(1, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.None);
                            break;
                        case "Bank 1 Left":
                            boostMovement = new BankBoost(1, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.None);
                            break;
                        case "Bank 1 Right":
                            boostMovement = new BankBoost(1, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.None);
                            break;
                        case "Turn 1 Right":
                            boostMovement = new TurnBoost(1, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.None);
                            break;
                        case "Turn 1 Left":
                            boostMovement = new TurnBoost(1, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.None);
                            break;
                        default:
                            boostMovement = new StraightBoost(1, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.None);
                            break;
                    }

                    // Predict our collisions and future position.
                    boostPrediction = new MovementPrediction(boostMovement);

                    VirtualBoard.SetVirtualPositionInfo(thisShip, boostPrediction.FinalPositionInfo);

                    if(isBeforeManeuverPhase == true)
                    {
                        // We need to now perform our maneuver from this new position.
                        MovementPrediction maneuverAfterAction = new MovementPrediction(thisShip.AssignedManeuver);
                        VirtualBoard.SetVirtualPositionInfo(thisShip, maneuverAfterAction.FinalPositionInfo);
                    }
                    // Find out how good this move is.
                    currentBoostNavigation = GetCurrentPositionNavigationInfo(thisShip);
                    currentBoostResult = CalculatePositionPriority(currentBoostNavigation);

                    if(move.IsRed == true)
                    {
                        // Make red maneuvers a little less optimal.
                        currentBoostResult -= 250;
                    }
                    if (currentBoostResult > bestBoostResult)
                    {
                        // We have a new best boost result.
                        bestBoostResult = currentBoostResult;
                        bestBoostNavigation = currentBoostNavigation;
                        bestBoostMove = boostMovement;
                    }

                    // Reset our ship position for the next boost test.
                    VirtualBoard.SetVirtualPositionInfo(thisShip, shipOriginalPosition);
                }
                if(bestBoostResult > startingResult)
                {
                    result = bestBoostResult;
                    FinalSuggestion = bestBoostMove;
                }
            }
      
            // Restore our original move.
            if (savedMovement != null)
            {
                thisShip.SetAssignedManeuver(savedMovement, isSilent: true);
            }
            else
            {
                thisShip.ClearAssignedManeuver();
            }

            // Put us back to our normal location.
            VirtualBoard.SwitchToRealPosition(thisShip);
            return result;
        }

        // Set navigation information for the current ship's position.
        private static NavigationResult GetCurrentPositionNavigationInfo(GenericShip thisShip)
        {
            NavigationResult currentNavigationResult = new NavigationResult()
            {
                movement = thisShip.AssignedManeuver,
                distanceToNearestEnemy = 0,
                distanceToNearestEnemyInShotRange = 0,
                enemiesInShotRange = 0,
                isBumped = thisShip.IsBumped,
                isLandedOnObstacle = thisShip.IsLandedOnObstacle,
                obstaclesHit = 0,
                isOffTheBoard = false,
                minesHit = 0,
                isOffTheBoardNextTurn = false,
                isHitAsteroidNextTurn = false,
                FinalPositionInfo = thisShip.GetPositionInfo()
            };

            int enemiesInShotRange = 0;

            float minDistanceToNearestEnemyInShotRange = 0;
            foreach (GenericShip enemyShip in thisShip.Owner.EnemyShips.Values)
            {
                // Get our weapon shot info for each arc that has a weapon pointing in it.
                foreach (IShipWeapon currentWeapon in thisShip.GetAllWeapons())
                {
                    ShotInfo shotInfo = new ShotInfo(thisShip, enemyShip, currentWeapon);
                    if (shotInfo.IsShotAvailable)
                    {
                        // We only need to find one target in range to pass our requirements (at least one shot available against that enemy with any particular weapon).
                        enemiesInShotRange++;
                        if (minDistanceToNearestEnemyInShotRange < shotInfo.DistanceReal) minDistanceToNearestEnemyInShotRange = shotInfo.DistanceReal;
                        break;
                    }
                }
            }

            currentNavigationResult.enemiesInShotRange = enemiesInShotRange;
            currentNavigationResult.distanceToNearestEnemyInShotRange = minDistanceToNearestEnemyInShotRange;

            // Find the nearest distance to an enemy ship.
            float minDistanceToEnemyShip = float.MaxValue;
            foreach (GenericShip enemyShip in thisShip.Owner.EnemyShips.Values)
            {
                DistanceInfo distInfo = new DistanceInfo(CurrentShip, enemyShip);
                if (distInfo.MinDistance.DistanceReal < minDistanceToEnemyShip) minDistanceToEnemyShip = distInfo.MinDistance.DistanceReal;
            }

            currentNavigationResult.distanceToNearestEnemy = minDistanceToEnemyShip;

            return currentNavigationResult;
        }

        // Determine how good the position we have been passed is.
        private static int CalculatePositionPriority(NavigationResult CurrentPosition)
        {
            int Priority = 0;

            if (CurrentPosition.isOffTheBoard)
            {
                return 0;
            }
            if (CurrentPosition.isLandedOnObstacle) Priority -= 10000;

            if (CurrentPosition.isOffTheBoardNextTurn) Priority -= 20000;

            Priority += CurrentPosition.enemiesInShotRange * 1000;

            Priority -= CurrentPosition.obstaclesHit * 2000;
            Priority -= CurrentPosition.minesHit * 2000;

            if (CurrentPosition.isHitAsteroidNextTurn) Priority -= 1000;

            if (CurrentPosition.isBumped)
            {
                // Leave space for testing Arvyl and Zeb.
                Priority -= 1000;
            }

            //distance is 0..10
            Priority += (10 - (int) CurrentPosition.distanceToNearestEnemy) * 10;
            return Priority;
        }
    }
}

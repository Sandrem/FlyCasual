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
            int enemiesThatCanShootUs = 0;
            float minDistanceToNearestEnemyInShotRange = 0;
            ShotInfo enemyCanShootThisShip = null;
            foreach (GenericShip enemyShip in CurrentShip.Owner.EnemyShips.Values)
            {
                // See if we can shoot this enemy.
                ShotInfo shotInfo = new ShotInfo(CurrentShip, enemyShip, CurrentShip.PrimaryWeapons.First());
                if (shotInfo.IsShotAvailable)
                {
                    enemiesInShotRange++;
                    if (minDistanceToNearestEnemyInShotRange < shotInfo.DistanceReal) minDistanceToNearestEnemyInShotRange = shotInfo.DistanceReal;


                }
                // See if this enemy can shoot us.
                enemyCanShootThisShip = new ShotInfo(enemyShip, CurrentShip, enemyShip.PrimaryWeapons.First());
                if (enemyCanShootThisShip.IsShotAvailable == true)
                {
                    enemiesThatCanShootUs++;
                }
            }

            NavigationResult result = new NavigationResult()
            {
                movement = CurrentMovementPrediction.CurrentMovement,
                distanceToNearestEnemy = minDistanceToEnenmyShip,
                distanceToNearestEnemyInShotRange = minDistanceToNearestEnemyInShotRange,
                enemiesInShotRange = enemiesInShotRange,
                enemiesTargetingThisShip = enemiesThatCanShootUs,
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
        public static int TryActionPossibilities(GenericAction actionToTry, bool isBeforeManeuverPhase = false)
        {
            VirtualBoard myBoard = new VirtualBoard();
            int result = 0;
            GenericShip thisShip = Selection.ActiveShip;
            GenericMovement savedMovement = thisShip.AssignedManeuver;
            int shieldHullTotal = thisShip.State.ShieldsCurrent + thisShip.State.HullCurrent;
            int startingResult = 0;

            NavigationResult StartingPosition = null;
            ShipPositionInfo shipOriginalPosition = thisShip.GetPositionInfo();

            // Prepare our virtual board maneuver tests.
            myBoard.UpdatePositionInfo(thisShip);

            // Record our current position for comparison.
            if (isBeforeManeuverPhase == true)
            {
                // Our action is before a maneuver.  Find out the results for our maneuver if we don't boost/barrel-roll/decloak before it.
                MovementPrediction maneuverWithoutActionFirst = new MovementPrediction(thisShip.AssignedManeuver);
                myBoard.SetVirtualPositionInfo(thisShip, maneuverWithoutActionFirst.FinalPositionInfo);
                myBoard.SwitchToVirtualPosition(thisShip);
                StartingPosition = GetCurrentPositionNavigationInfo(thisShip, maneuverWithoutActionFirst);
                myBoard.SwitchToRealPosition(thisShip);

                // Move us back to before the maneuver.
                myBoard.SetVirtualPositionInfo(thisShip, shipOriginalPosition);
            }
            else
            {
                // Just record our current position.
                myBoard.SwitchToVirtualPosition(thisShip);
                StartingPosition = GetCurrentPositionNavigationInfo(thisShip);
                myBoard.SwitchToRealPosition(thisShip);
            }

            // Test for a boost action.
            if (actionToTry is BoostAction)
            {
                // Determine how good our starting position is.
                startingResult = CalculateBoostPositionPriority(StartingPosition);
                result = TryBoostAction(myBoard, thisShip, shipOriginalPosition, StartingPosition, startingResult, isBeforeManeuverPhase);
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

            return result;
        }

        // Set navigation information for the current ship's position.
        private static NavigationResult GetCurrentPositionNavigationInfo(GenericShip thisShip, MovementPrediction firstMovement = null, MovementPrediction secondMovement = null)
        {
            NavigationResult currentNavigationResult = new NavigationResult()
            {
                movement = thisShip.AssignedManeuver,
                distanceToNearestEnemy = 0,
                distanceToNearestEnemyInShotRange = 0,
                enemiesInShotRange = 0,
                enemiesTargetingThisShip = 0,
                isBumped = thisShip.IsBumped,
                isLandedOnObstacle = thisShip.IsLandedOnObstacle,
                obstaclesHit = 0,
                isOffTheBoard = false,
                minesHit = 0,
                isOffTheBoardNextTurn = false,
                isHitAsteroidNextTurn = false,
                FinalPositionInfo = thisShip.GetPositionInfo()
            };

            // Duplicate all information we can from our movement prediction(s).
            if(firstMovement != null)
            {
                if(secondMovement != null)
                {
                    currentNavigationResult.movement = secondMovement.CurrentMovement;
                    currentNavigationResult.isBumped = (secondMovement.IsBumped || firstMovement.IsBumped);
                    currentNavigationResult.isLandedOnObstacle = (secondMovement.IsLandedOnAsteroid || firstMovement.IsLandedOnAsteroid);
                    currentNavigationResult.obstaclesHit = (secondMovement.AsteroidsHit.Count + firstMovement.AsteroidsHit.Count);
                    currentNavigationResult.isOffTheBoard = (secondMovement.IsOffTheBoard || firstMovement.IsOffTheBoard);
                    currentNavigationResult.minesHit = (secondMovement.MinesHit.Count + firstMovement.MinesHit.Count);
                }
                else
                {
                    currentNavigationResult.movement = firstMovement.CurrentMovement;
                    currentNavigationResult.isBumped = firstMovement.IsBumped;
                    currentNavigationResult.isLandedOnObstacle = firstMovement.IsLandedOnAsteroid;
                    currentNavigationResult.obstaclesHit = firstMovement.AsteroidsHit.Count;
                    currentNavigationResult.isOffTheBoard = firstMovement.IsOffTheBoard;
                    currentNavigationResult.minesHit = firstMovement.MinesHit.Count;
                }
            }

            int enemiesInShotRange = 0;

            float minDistanceToNearestEnemyInShotRange = float.MaxValue;
            CurrentShip = thisShip;
            foreach (GenericShip enemyShip in thisShip.Owner.EnemyShips.Values)
            {
                if (IsActivationBeforeCurrentShip(enemyShip))
                {
                    // This enemy acts before us.  They won't be able to leave our targeting arc.
                    // Get our weapon shot info for each arc that has a weapon pointing in it.
                    foreach (IShipWeapon currentWeapon in thisShip.GetAllWeapons())
                    {
                        ShotInfo shotInfo = new ShotInfo(thisShip, enemyShip, currentWeapon);
                        if (shotInfo.IsShotAvailable)
                        {
                            // We are looking for our closest enemy target.
                            enemiesInShotRange++;
                            if (minDistanceToNearestEnemyInShotRange > shotInfo.DistanceReal) minDistanceToNearestEnemyInShotRange = shotInfo.DistanceReal;
                        }
                    }
                }
            }

            currentNavigationResult.enemiesInShotRange = enemiesInShotRange;
            currentNavigationResult.distanceToNearestEnemyInShotRange = minDistanceToNearestEnemyInShotRange;

            // Find the nearest distance to an enemy ship.
            float minDistanceToEnemyShip = float.MaxValue;
            ShotInfo enemyCanShootThisShip = null;
            foreach (GenericShip enemyShip in thisShip.Owner.EnemyShips.Values)
            {
                DistanceInfo distInfo = new DistanceInfo(CurrentShip, enemyShip);
                if (distInfo.MinDistance.DistanceReal < minDistanceToEnemyShip)
                {
                    if(distInfo.MinDistance.DistanceReal != 0 || thisShip.IsBumped == true)
                    {
                        // A value of 0 is false unless we've bumped. 
                        minDistanceToEnemyShip = distInfo.MinDistance.DistanceReal;
                    }

                }

                // See if this enemy can shoot us with any of its weapons.
                foreach (IShipWeapon currentWeapon in thisShip.GetAllWeapons())
                {
                    enemyCanShootThisShip = new ShotInfo(enemyShip, thisShip, currentWeapon);
                    if (enemyCanShootThisShip.IsShotAvailable == true)
                    {
                        currentNavigationResult.enemiesTargetingThisShip++;
                        break;
                    }
                }
            }

            currentNavigationResult.distanceToNearestEnemy = minDistanceToEnemyShip;

            return currentNavigationResult;
        }

        // Determine how good the position we have been passed is.
        private static int CalculateBoostPositionPriority(NavigationResult CurrentPosition)
        {
            int Priority = 0;
            if (CurrentPosition.isOffTheBoard)
            {
                return 0;
            }
            if (CurrentPosition.isLandedOnObstacle) Priority -= 20000;

            if (CurrentPosition.isOffTheBoardNextTurn) Priority -= 20000;

            // Base our priority off of how many enemies can shoot us versus how many we can shoot.
            if (CurrentPosition.enemiesInShotRange > 0)
            {
                // We have at least one enemy in shot range.  We don't need to maximize our targets.
                Priority += 20;
                if(CurrentPosition.distanceToNearestEnemyInShotRange < 1)
                {
                    // We are at range 1 of a target?
                    Priority += 10;
                }
            }
            // Reduce our priority by the number of enemies that can still target us after the action.
            Priority -= CurrentPosition.enemiesTargetingThisShip * 40;
            if(CurrentPosition.enemiesTargetingThisShip == 0)
            {
                // Any position that leads to no-one attacking us is a pretty good position.
                Priority += 10;
                if(CurrentPosition.enemiesInShotRange > 0)
                {
                    // We have no-one targeting us and at least one target in range.  A really good action!
                    Priority += 30;
                }
            }

            if (CurrentPosition.obstaclesHit > 0)
            {
                Priority -= CurrentPosition.obstaclesHit * 2000;
            }
            Priority -= CurrentPosition.minesHit * 2000;

            if (CurrentPosition.isBumped)
            {
                // Leave space for testing Arvyl and Zeb.  Otherwise, we want to avoid this.
                Priority -= 1000;
            }

            // Set our priority between 0 and 90.
            if(Priority < 0)
            {
                Priority = 0;
            }

            return Priority;
        }

        // Given a ship, their starting position, their current navigation results and its priority, determine which, if any, boost action is best for us.  If this action
        // takes place before the maneuver phase, calculate our results based on our chosen maneuver.
        private static int TryBoostAction(VirtualBoard myBoard, GenericShip thisShip, ShipPositionInfo shipOriginalPosition, NavigationResult StartingPosition, int startingResult, bool isBeforeManeuverPhase = false)
        {
            int result = 0;
            int bestBoostResult = 0;
            string bestBoostName = "1.S";
            GenericMovement bestBoostMove = null;
            NavigationResult bestBoostNavigation = null;
            AiSinglePlan bestPlan = new AiSinglePlan();

            NavigationResult currentBoostNavigation = null;
            int currentBoostResult = 0;
            bool bestMoveStresses = false;

            // We're performing a boost action.  Check all boost action possibilities.
            List<BoostMove> AvailableBoostMoves = new List<BoostMove>();
            AvailableBoostMoves = thisShip.GetAvailableBoostTemplates();
            int numBoostMoves = AvailableBoostMoves.Count();
 
            foreach (BoostMove move in AvailableBoostMoves)
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
                        boostMovement = new BankBoost(1, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.None); ;
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
                boostMovement.Initialize();
                boostPrediction = new MovementPrediction(boostMovement);
                boostPrediction.CalculateMovePredictionFast();

                myBoard.SetVirtualPositionInfo(thisShip, boostPrediction.FinalPositionInfo);
                MovementPrediction maneuverAfterAction = null;
                if (isBeforeManeuverPhase == true)
                {
                    // We need to now perform our maneuver from this new position.
                    maneuverAfterAction = new MovementPrediction(thisShip.AssignedManeuver);
                    maneuverAfterAction.CalculateMovePredictionFast();
                    myBoard.SetVirtualPositionInfo(thisShip, maneuverAfterAction.FinalPositionInfo);
                }

                // Find out how good this move is.
                myBoard.SwitchToVirtualPosition(thisShip);

                currentBoostNavigation = GetCurrentPositionNavigationInfo(thisShip, boostPrediction, maneuverAfterAction);
                myBoard.SwitchToRealPosition(thisShip);

                if (currentBoostNavigation == null)
                {
                    currentBoostResult = -20000;
                }
                currentBoostResult = CalculateBoostPositionPriority(currentBoostNavigation);

                if (move.IsRed == true)
                {
                    // Make red maneuvers a little less optimal.
                    currentBoostResult -= 10;
                }

                if (currentBoostResult > bestBoostResult)
                {
                    // We have a new best boost result.
                    bestBoostResult = currentBoostResult;
                    bestBoostNavigation = currentBoostNavigation;
                    bestBoostMove = boostMovement;
                    bestMoveStresses = move.IsRed;
                    bestBoostName = selectedBoostHelper;
                }


                // Reset our ship position for the next boost test.
                myBoard.SetVirtualPositionInfo(thisShip, shipOriginalPosition);
            }
            if (bestBoostResult > startingResult)
            {
                // Boosting is better than staying here!
                // Since the navigation results can reach a maximum of 8,000 (8 enemies in range, none targeting us), we need the range to be up to 100 to match other priorities.

                result = bestBoostResult;
                bestPlan.Priority = bestBoostResult;
                bestPlan.currentAction = new BoostAction();
                bestPlan.currentActionMove = bestBoostMove;
                bestPlan.actionName = bestBoostName;
                bestPlan.isRedAction = bestMoveStresses;

                // Give our ship our new plan.
                thisShip.AiPlans.AddPlan(bestPlan);
            }

            return result;
        }
    }
}

using SubPhases;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Players
{

    public partial class HotacAiPlayer : GenericAiPlayer
    {
        public HotacAiPlayer() : base()
        {
            Name = "HotAC AI";
        }

        public override void ActivateShip(Ship.GenericShip ship)
        {
            Console.Write(ship.PilotName + " (" + ship.ShipId + ") is activated to perform maneuver", LogTypes.AI);

            bool isTargetLockPerformed = false;
            Ship.GenericShip targetShip;

            if (ship.Formation != null && ship.Formation.CurrentManeuver != null)
            {
                ship.SetAssignedManeuver(ShipMovementScript.MovementFromString(ship.Formation.CurrentManeuver));
                targetShip = ship.Formation.CurrentTarget;
                Console.Write("Using maneuver and target from formation:" + targetShip.PilotName + " (" + targetShip.ShipId + ")", LogTypes.AI);
            }
            else
            {
                targetShip = FindNearestEnemyShip(ship, ignoreCollided: true, inArcAndRange: true);
                if (targetShip == null) targetShip = FindNearestEnemyShip(ship, ignoreCollided: true);
                if (targetShip == null) targetShip = FindNearestEnemyShip(ship);
                Console.Write("Nearest enemy is " + targetShip.PilotName + " (" + targetShip.ShipId + ")", LogTypes.AI);

                Movement.GenericMovement selectedManeuver;

                // TODO: remove null variant

                if (!RulesList.IonizationRule.IsIonized(ship))
                {
                    if (targetShip != null)
                    {
                        selectedManeuver = ship.HotacManeuverTable.GetManeuver(ship, targetShip);
                    }
                    else
                    {
                        selectedManeuver = new Movement.StraightMovement(2, Movement.ManeuverDirection.Forward, Movement.ManeuverBearing.Straight, Movement.MovementComplexity.Normal);
                    }

                    ship.SetAssignedManeuver(selectedManeuver);

                    if (ship.Formation != null && ship.Formation.CurrentManeuver == null)
                    {
                        ship.Formation.SetManeuverAndTarget(selectedManeuver.ToString(), targetShip);
                        Console.Write(ship.PilotName + " (" + ship.ShipId + ") is setting maneuver for formation", LogTypes.AI);
                    }
                }
            }

            if (targetShip != null) foreach (var action in ship.GetAvailableActions())
            {
                if (action.GetType() == typeof(ActionsList.TargetLockAction))
                {
                    isTargetLockPerformed = true;
                    Actions.AcquireTargetLock(
                        ship,
                        targetShip,
                        delegate { PerformManeuverOfShip(ship); },
                        delegate { PerformManeuverOfShip(ship); }
                    );
                    break;
                }
            }

            if (!isTargetLockPerformed)
            {
                PerformManeuverOfShip(ship);
            }
        }

        public override void TakeDecision()
        {
            if (Phases.CurrentSubPhase is ActionDecisonSubPhase)
            {
                PerformActionFromList(Selection.ThisShip.GetAvailableActions());
            }
            else if (Phases.CurrentSubPhase is FreeActionDecisonSubPhase)
            {
                PerformActionFromList(Selection.ThisShip.GetAvailableFreeActions());
            }
            else (Phases.CurrentSubPhase as DecisionSubPhase).DoDefault();
        }

        private void PerformActionFromList(List<ActionsList.GenericAction> actionsList)
        {
            bool isActionTaken = false;

            if (Selection.ThisShip.Tokens.GetToken(typeof(Tokens.StressToken)) != null)
            {
                isActionTaken = true;
                Selection.ThisShip.Tokens.RemoveToken(
                    typeof(Tokens.StressToken),
                    Phases.CurrentSubPhase.CallBack
                );
            }
            else
            {
                List<ActionsList.GenericAction> availableActionsList = actionsList;

                Dictionary<ActionsList.GenericAction, int> actionsPriority = new Dictionary<ActionsList.GenericAction, int>();

                foreach (var action in availableActionsList)
                {
                    int priority = action.GetActionPriority();
                    actionsPriority.Add(action, priority);
                }

                actionsPriority = actionsPriority.OrderByDescending(n => n.Value).ToDictionary(n => n.Key, n => n.Value);

                if (actionsPriority.Count > 0)
                {
                    KeyValuePair<ActionsList.GenericAction, int> prioritizedActions = actionsPriority.First();
                    if (prioritizedActions.Value > 0)
                    {
                        isActionTaken = true;
                        Actions.TakeActionStart(prioritizedActions.Key);
                    }
                }
            }

            if (!isActionTaken) Phases.CurrentSubPhase.CallBack();
        }

        public override void AfterShipMovementPrediction()
        {
            if (Selection.ThisShip.AssignedManeuver.IsRevealDial)
            {
                bool leaveMovementAsIs = true;

                if (Selection.ThisShip.AssignedManeuver.movementPrediction.IsOffTheBoard)
                {
                    leaveMovementAsIs = false;
                    Console.Write("Ship predicts off the board maneuver!", LogTypes.AI);
                    AvoidOffTheBoard();
                }
                else
                {
                    if (Selection.ThisShip.AssignedManeuver.movementPrediction.AsteroidsHit.Count != 0)
                    {
                        leaveMovementAsIs = false;
                        Console.Write("Ship predicts collision with asteroid!", LogTypes.AI);
                        Swerve();
                    }
                }

                if (leaveMovementAsIs)
                {
                    Console.Write("Ship executes selected maneuver\n", LogTypes.AI, true);
                    Selection.ThisShip.AssignedManeuver.LaunchShipMovement();
                }
            }
            else
            {
                Console.Write("Ship is ionized and doesn't select maneuver\n", LogTypes.AI, true);
                Selection.ThisShip.AssignedManeuver.LaunchShipMovement();
            }
        }

        private void Swerve()
        {
            new AI.Swerve();
        }

        private void AvoidOffTheBoard()
        {
            new AI.Swerve(true);
        }

    }
}

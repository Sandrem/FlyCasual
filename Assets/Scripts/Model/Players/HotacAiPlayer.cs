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

            Ship.GenericShip anotherShip = FindNearestEnemyShip(ship, ignoreCollided: true, inArcAndRange: true);
            if (anotherShip == null) anotherShip = FindNearestEnemyShip(ship, ignoreCollided: true);
            if (anotherShip == null) anotherShip = FindNearestEnemyShip(ship);
            Console.Write("Nearest enemy is " + ship.PilotName + " (" + ship.ShipId + ")", LogTypes.AI);

            // TODO: remove null variant
            if (anotherShip != null)
            {
                ship.SetAssignedManeuver(ship.HotacManeuverTable.GetManeuver(ship, anotherShip));
            }
            else
            {
                ship.SetAssignedManeuver(new Movement.StraightMovement(2, Movement.ManeuverDirection.Forward, Movement.ManeuverBearing.Straight, Movement.ManeuverColor.White));
            }

            ship.GenerateAvailableActionsList();
            if (anotherShip != null) foreach (var action in ship.GetAvailableActionsList())
            {
                if (action.GetType() == typeof(ActionsList.TargetLockAction))
                {
                    isTargetLockPerformed = true;
                    Actions.AcquireTargetLock(
                        ship,
                        anotherShip,
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

        public override void PerformAction()
        {
            PerformActionFromList(Selection.ThisShip.GetAvailableActionsList());
        }

        public override void PerformFreeAction()
        {
            PerformActionFromList(Selection.ThisShip.GetAvailableFreeActionsList());
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
                        Selection.ThisShip.AddAlreadyExecutedAction(prioritizedActions.Key);
                        prioritizedActions.Key.ActionTake();
                    }
                }
            }

            if (!isActionTaken) Phases.CurrentSubPhase.CallBack();
        }

        public override void AfterShipMovementPrediction()
        {
            if (Selection.ThisShip.AssignedManeuver.IsRealMovement)
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

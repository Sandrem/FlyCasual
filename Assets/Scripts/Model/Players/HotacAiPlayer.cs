using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Players
{

    public partial class HotacAiPlayer : GenericAiPlayer
    {
        private bool inDebug = false;

        public HotacAiPlayer() : base()
        {
            Name = "HotAC AI";
        }

        public override void ActivateShip(Ship.GenericShip ship)
        {
            if (inDebug) Debug.Log("=== " + ship.PilotName + " (" + ship.ShipId + ") ===");

            bool isTargetLockPerformed = false;

            Ship.GenericShip anotherShip = FindNearestEnemyShip(ship, ignoreCollided: true, inArcAndRange: true);
            if (anotherShip == null) anotherShip = FindNearestEnemyShip(ship, ignoreCollided: true);
            if (anotherShip == null) anotherShip = FindNearestEnemyShip(ship);
            if (inDebug) Debug.Log("Nearest enemy is " + anotherShip.PilotName + " (" + anotherShip.ShipId + ")");

            // TODO: remove null variant
            if (anotherShip != null)
            {
                ship.AssignedManeuver = ship.HotacManeuverTable.GetManeuver(ship, anotherShip);
            }
            else
            {
                ship.AssignedManeuver = new Movement.StraightMovement(2, Movement.ManeuverDirection.Forward, Movement.ManeuverBearing.Straight, Movement.ManeuverColor.White);
            }

            ship.GenerateAvailableActionsList();
            if (anotherShip != null) foreach (var action in ship.GetAvailableActionsList())
            {
                if (action.GetType() == typeof(ActionsList.TargetLockAction))
                {
                    isTargetLockPerformed = true;
                    Actions.AssignTargetLockToPair(
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
            bool isActionTaken = false;

            if (Selection.ThisShip.GetToken(typeof(Tokens.StressToken)) != null)
            {
                Selection.ThisShip.RemoveToken(typeof(Tokens.StressToken));
            }
            else
            {
                List<ActionsList.GenericAction> availableActionsList = Selection.ThisShip.GetAvailableActionsList();

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
            bool leaveMovementAsIs = true;

            if (Selection.ThisShip.AssignedManeuver.movementPrediction.IsOffTheBoard)
            {
                leaveMovementAsIs = false;
                if (DebugManager.DebugAI) Debug.Log("AI predicts off the board maneuver!");
                AvoidOffTheBoard();
            }
            else
            {
                if (Selection.ThisShip.AssignedManeuver.movementPrediction.AsteroidsHit.Count != 0)
                {
                    leaveMovementAsIs = false;
                    if (DebugManager.DebugAI) Debug.Log("AI predicts asteroid hit!");
                    Swerve();
                }
            }

            if (leaveMovementAsIs) Selection.ThisShip.AssignedManeuver.LaunchShipMovement();
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

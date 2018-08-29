using GameModes;
using Ship;
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
            UsesHotacAiRules = true;
        }

        public override void ActivateShip(GenericShip ship)
        {
            Console.Write(ship.PilotName + " (" + ship.ShipId + ") is activated to perform maneuver", LogTypes.AI);

            GenericShip anotherShip = FindNearestEnemyShip(ship, ignoreCollided: true, inArcAndRange: true);
            if (anotherShip == null) anotherShip = FindNearestEnemyShip(ship, ignoreCollided: true);
            if (anotherShip == null) anotherShip = FindNearestEnemyShip(ship);
            Console.Write("Nearest enemy is " + ship.PilotName + " (" + ship.ShipId + ")", LogTypes.AI);

            // TODO: remove null variant

            if (!RulesList.IonizationRule.IsIonized(ship) && (anotherShip != null))
            {
                ship.SetAssignedManeuver(ship.HotacManeuverTable.GetManeuver(ship, anotherShip));
            }

            TryPerformFreeTargetLock(ship, anotherShip);
        }

        private void TryPerformFreeTargetLock(GenericShip ship, GenericShip anotherShip)
        {
            bool isTargetLockPerformed = false;

            if (anotherShip != null && ship.GetAvailableActions().Any(a => a.GetType() == typeof(ActionsList.TargetLockAction)))
            {
                isTargetLockPerformed = true;

                JSONObject parameters = new JSONObject();
                parameters.AddField("id", ship.ShipId.ToString());
                parameters.AddField("target", anotherShip.ShipId.ToString());
                GameController.SendCommand(
                    GameCommandTypes.HotacFreeTargetLock,
                    Phases.CurrentSubPhase.GetType(),
                    parameters.ToString()
                );

                PerformManeuverOfShip(ship);
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

                        //Actions.TakeActionStart(prioritizedActions.Key);
                        JSONObject parameters = new JSONObject();
                        parameters.AddField("name", prioritizedActions.Key.Name);
                        GameController.SendCommand(
                            GameCommandTypes.Decision,
                            Phases.CurrentSubPhase.GetType(),
                            parameters.ToString()
                        );
                    }
                }
            }

            if (!isActionTaken)
            {
                GameMode.CurrentGameMode.ExecuteCommand(UI.GenerateSkipButtonCommand());
            }
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

                    AI.Swerve.GenerateSwerveCommand(Selection.ThisShip.ShipId, Selection.ThisShip.AssignedManeuver.ToString());
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

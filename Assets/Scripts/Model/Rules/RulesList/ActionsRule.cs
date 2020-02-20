
using Actions;
using ActionsList;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using UnityEngine;

namespace RulesList
{
    public class ActionsRule
    {

        public void CanPerformActions(GenericShip ship, GenericAction action, ref bool result)
        {
            if (ship.IsAlreadyExecutedAction(action)) result = false;

            if (action.Color == ActionColor.Purple && ship.State.Force == 0) result = false;
        }

        public void ActionColorCheck(GenericAction action)
        {
            // Selection.ThisShip is null during tractor beam
            if (action == null || Selection.ThisShip == null) return;

            ActionColor color = action.Color;
            color = Selection.ThisShip.CallOnCheckActionComplexity(action, ref color);

            //AI perfroms red actions as white, because it is hard to calculate correct priority of red action
            if (color == ActionColor.Red && !(Selection.ThisShip.Owner is Players.GenericAiPlayer))
            {
                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Stress after red action",
                    TriggerType = TriggerTypes.OnActionIsPerformed_System,
                    TriggerOwner = Selection.ThisShip.Owner.PlayerNo,
                    EventHandler = GetStress
                });
            }

            if (color == ActionColor.Purple) Selection.ThisShip.State.Force--;
        }

        private void GetStress(object sender, System.EventArgs e)
        {
            Selection.ThisShip.Tokens.AssignToken(typeof(StressToken), Triggers.FinishTrigger);
        }

        public void CheckLinkedAction(GenericAction action)
        {
            // Selection.ThisShip is null during tractor beam
            if (action == null || Selection.ThisShip == null) return;

            List<GenericAction> possibleLinkedActions = new List<GenericAction>();

            foreach(KeyValuePair<Type, GenericAction> linkedAction in Selection.ThisShip.ActionBar.LinkedActions)
            {
                if (linkedAction.Key == action.GetType())
                    possibleLinkedActions.Add(linkedAction.Value);
            }

            if (possibleLinkedActions.Count > 0)
            {
                Selection.ThisShip.PlannedLinkedActions = possibleLinkedActions;
                Selection.ThisShip.OnActionDecisionSubphaseEnd += DoSecondAction;
            }
        }

        private void DoSecondAction(GenericShip ship)
        {
            Selection.ThisShip.OnActionDecisionSubphaseEnd -= DoSecondAction;

            Triggers.RegisterTrigger(
                new Trigger()
                {
                    Name = "Execute a linked action",
                    TriggerOwner = ship.Owner.PlayerNo,
                    TriggerType = TriggerTypes.OnFreeAction,
                    EventHandler = PerformLinkedAction
                }
            );
        }

        private void PerformLinkedAction(object sender, System.EventArgs e)
        {
            Selection.ThisShip.GenerateAvailableActionsList();
            Selection.ThisShip.AskPerformFreeAction(
                Selection.ThisShip.PlannedLinkedActions,
                Triggers.FinishTrigger,
                (Selection.ThisShip.PlannedLinkedActions.Count == 1) ? "Linked Action" : "Linked Actions"
            );
        }

        public static bool HasPerformActionStep(GenericShip ship)
        {
            if (ship.IsDestroyed) return false;
            if (ship.IsSkipsActionSubPhase) return false;
            
            if (ship.Tokens.HasToken(typeof(StressToken)))
            {
                if ((!ship.CallCheckCanPerformActionsWhileStressed()) && (!ship.GetAvailableActions().Any(n => n.CanBePerformedWhileStressed)) && (ship.ActionBar.ActionsThatCanbePreformedwhileStressed.Count == 0)) return false;
            }

            return true;
        }

        public void ActionIsFailed(GenericShip ship, GenericAction action, ActionFailReason failReason, bool hasSecondChance = false)
        {
            ActionIsFailed(ship, action, new List<ActionFailReason>() { failReason }, hasSecondChance);
        }

        public void ActionIsFailed(GenericShip ship, GenericAction action, List<ActionFailReason> failReasons, bool hasSecondChance = false)
        {
            ship.CallActionIsReadyToBeFailed(action, failReasons, hasSecondChance);
        }

    }
}


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

        public void CanPerformActions(GenericAction action, ref bool result)
        {
            if (Selection.ThisShip.IsAlreadyExecutedAction(action.GetType())) result = false;
        }

        public void RedActionCheck(GenericAction action)
        {
            if (action == null) return;

            //HotAC AI perfroms red actions as white
            if (action.IsRed && !Selection.ThisShip.Owner.UsesHotacAiRules)
            {
                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Stress after red action",
                    TriggerType = TriggerTypes.OnActionIsPerformed,
                    TriggerOwner = Selection.ThisShip.Owner.PlayerNo,
                    EventHandler = GetStress
                });
            }
        }

        private void GetStress(object sender, System.EventArgs e)
        {
            Selection.ThisShip.Tokens.AssignToken(typeof(StressToken), Triggers.FinishTrigger);
        }

        public void CheckLinkedAction(GenericAction action)
        {
            if (action == null) return;

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
            Selection.ThisShip.AskPerformFreeAction(Selection.ThisShip.PlannedLinkedActions, Triggers.FinishTrigger);
        }

        public static bool HasPerformActionStep(GenericShip ship)
        {
            if (ship.IsDestroyed) return false;
            if (ship.IsSkipsActionSubPhase) return false;
            
            if (ship.Tokens.HasToken(typeof(StressToken)))
            {
                if ((!ship.CanPerformActionsWhileStressed) && (!ship.GetAvailableActions().Any(n => n.CanBePerformedWhileStressed))) return false;
            }

            return true;
        }

    }
}


using ActionsList;
using Ship;
using System.Collections.Generic;
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

            if (action.IsRed)
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
            Selection.ThisShip.Tokens.AssignToken(new StressToken(Selection.ThisShip), Triggers.FinishTrigger);
        }

        public void CheckLinkedAction(GenericAction action)
        {
            if (action == null) return;

            if (action.LinkedRedAction != null)
            {
                Selection.ThisShip.PlannedLinkedAction = action.LinkedRedAction;
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
            List<GenericAction> linkedAction = new List<GenericAction>() { Selection.ThisShip.PlannedLinkedAction };
            Selection.ThisShip.AskPerformFreeAction(linkedAction, Triggers.FinishTrigger, true);
        }

    }
}

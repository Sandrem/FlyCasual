
using ActionsList;
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

    }
}

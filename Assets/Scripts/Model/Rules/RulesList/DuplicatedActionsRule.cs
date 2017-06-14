
using UnityEngine;

namespace RulesList
{
    public class DuplicatedActionsRule
    {

        public void CanPerformActions(ActionsList.GenericAction action, ref bool result)
        {
            if (Selection.ThisShip.IsAlreadyExecutedAction(action.GetType())) result = false;
        }

    }
}

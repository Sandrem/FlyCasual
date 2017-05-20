
using UnityEngine;

namespace RulesList
{
    public class DuplicatedActionsRule
    {

        public void CanPerformActions(ActionsList.GenericAction action, ref bool result)
        {
            if (Selection.ThisShip.AlreadyExecutedAction(action.GetType())) result = false;
        }

    }
}

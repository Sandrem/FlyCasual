using Actions;
using ActionsList;
using System;
using System.Collections.Generic;

namespace Abilities.Parameters
{
    public class ActionInfo
    {
        public Type ActionType { get; }
        public ActionColor ActionColor { get; }
        public bool CanBePerformedWhileStressed { get; }

        public ActionInfo(Type actionType, ActionColor actionColor = ActionColor.White, bool canBePerformedWhileStressed = false)
        {
            ActionType = actionType;
            ActionColor = actionColor;
            CanBePerformedWhileStressed = canBePerformedWhileStressed;
        }

        public GenericAction GenerateAction()
        {
            GenericAction action = (GenericAction)Activator.CreateInstance(ActionType);
            action.Color = ActionColor;
            if (CanBePerformedWhileStressed) action.CanBePerformedWhileStressed = true;
            return action;
        }
    }
}

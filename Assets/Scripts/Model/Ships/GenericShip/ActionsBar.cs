using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ship
{
    public class ActionsBar
    {
        public List<ActionsList.GenericAction> PrintedActionsbaActions { get; private set; }
        public List<ActionsList.GenericAction> ActionbarActions { get; private set; }

        public ActionsBar()
        {
            PrintedActionsbaActions = new List<ActionsList.GenericAction>();
            ActionbarActions = new List<ActionsList.GenericAction>();
        }

        public void AddActionbarAction(ActionsList.GenericAction action, bool isPrinted)
        {
            ActionbarActions.Add(action);
            if (isPrinted) PrintedActionsbaActions.Add(action);
        }

        public void RemoveActionbarAction(System.Type type)
        {
            ActionsList.GenericAction action = ActionbarActions.Find(n => n.GetType() == type);
            if (action != null) ActionbarActions.Remove(action);
        }
    }
}

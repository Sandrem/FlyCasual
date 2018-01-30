using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionsList;

namespace Ship
{
    public class ActionsBar
    {
        private List<GenericAction> PrintedActionbarActions;
        private List<GenericAction> ActionbarActions;

        public ActionsBar()
        {
            PrintedActionbarActions = new List<GenericAction>();
            ActionbarActions = new List<GenericAction>();
        }

        public void AddActionbarAction(GenericAction action, bool isPrinted)
        {
            ActionbarActions.Add(action);
            if (isPrinted) PrintedActionbarActions.Add(action);
        }

        public void RemoveActionbarAction(Type type)
        {
            GenericAction action = ActionbarActions.Find(n => n.GetType() == type);
            if (action != null) ActionbarActions.Remove(action);
        }
    }
}

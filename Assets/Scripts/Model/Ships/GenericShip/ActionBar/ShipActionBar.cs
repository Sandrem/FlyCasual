using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionsList;
using Upgrade;

namespace Ship
{
    public class ShipActionBar
    {
        private class AddedAction
        {
            public GenericAction Action;
            public GenericUpgrade Source;

            public AddedAction(GenericAction action, GenericUpgrade source)
            {
                Action = action;
                Source = source;
            }
        }

        private GenericShip Host;

        public List<GenericAction> AllActions
        {
            get
            {
                List<GenericAction> allActions = new List<GenericAction>();
                allActions.AddRange(PrintedActions);
                allActions.AddRange(AddedActions.Select(n => n.Action).ToList());
                return allActions;
            }
        }

        public List<GenericAction> PrintedActions;
        private List<AddedAction> AddedActions;

        public ShipActionBar(GenericShip host)
        {
            Host = host;

            PrintedActions = new List<GenericAction>();
            AddedActions = new List<AddedAction>();
        }

        public void AddPrintedAction(GenericAction action)
        {
            action.Host = Host;
            PrintedActions.Add(action);
        }

        public void AddGrantedAction(GenericAction action, GenericUpgrade source)
        {
            action.Host = Host;
            action.Source = source;
            AddedActions.Add(new AddedAction(action, source));
        }

        public void RemovePrintedAction(Type actionType)
        {
            GenericAction actionToRemove = PrintedActions.First(n => n.GetType() == actionType);
            PrintedActions.Remove(actionToRemove);
        }

        public void RemoveGrantedAction(Type actionType, GenericUpgrade source)
        {
            AddedAction actionToRemove = AddedActions.First(n => n.Action.GetType() == actionType && n.Source == source);
            AddedActions.Remove(actionToRemove);
        }

        public void RemoveGrantedAction(Type actionType, Type linkedRedAction, GenericUpgrade source)
        {
            AddedAction actionToRemove = AddedActions.First(n => n.Action.GetType() == actionType && n.Action.LinkedRedAction.GetType() == linkedRedAction && n.Source == source);
            AddedActions.Remove(actionToRemove);
        }

        public bool HasAction(Type type)
        {
            return AllActions.Any(n => n.GetType() == type);
        }

        public bool HasAction(Type type, bool isRed)
        {
            return AllActions.Any(n => n.GetType() == type && n.IsRed == isRed);
        }
    }
}

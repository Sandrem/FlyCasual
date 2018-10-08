using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionsList;
using UnityEngine;
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
        public List<KeyValuePair<Type, GenericAction>> LinkedActions;
        private List<AddedAction> AddedActions;

        public ShipActionBar(GenericShip host)
        {
            Host = host;

            PrintedActions = new List<GenericAction>();
            AddedActions = new List<AddedAction>();
            LinkedActions = new List<KeyValuePair<Type, GenericAction>>();
        }

        public void AddPrintedAction(GenericAction action)
        {
            action.Host = Host;
            action.IsInActionBar = true;
            PrintedActions.Add(action);
        }

        public void AddActionLink(Type actionType, GenericAction link, GenericUpgrade source = null)
        {
            LinkedActions.Add(new KeyValuePair<Type, GenericAction>(actionType, link));
        }

        public void RemoveActionLink(Type actionType, Type linkedRedAction, GenericUpgrade source = null)
        {
            KeyValuePair<Type, GenericAction> linkToRemove = LinkedActions.FirstOrDefault(n => n.Key == actionType && n.Value.GetType() == linkedRedAction && n.Value.Source == source);
            LinkedActions.Remove(linkToRemove);
        }

        public void AddGrantedAction(GenericAction action, GenericUpgrade source)
        {
            action.Host = Host;
            action.Source = source;
            action.IsInActionBar = true;
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

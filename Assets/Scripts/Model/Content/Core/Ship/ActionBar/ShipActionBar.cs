using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionsList;
using UnityEngine;
using Upgrade;
using Actions;

namespace Ship
{
    public class ShipActionsInfo
    {
        public List<ActionInfo> Actions { get; private set; }
        public List<LinkedActionInfo> LinkedActions { get; private set; }

        public ShipActionsInfo(params ActionInfo[] actions)
        {
            Actions = actions.ToList();
            LinkedActions = new List<LinkedActionInfo>();
        }

        public void AddActions(params ActionInfo[] actions)
        {
            Actions.AddRange(actions.ToList());
        }

        public void RemoveActions(params Type[] actionTypes)
        {
            Actions.RemoveAll(a => actionTypes.Contains(a.ActionType));
        }

        public void AddLinkedAction(params LinkedActionInfo[] linkedActions)
        {
            LinkedActions.AddRange(linkedActions.ToList());
        }

        public void RemoveLinkedAction(Type actionType, Type linkedActionType)
        {
            LinkedActions.RemoveAll(a => a.ActionType == actionType && a.ActionLinkedType == linkedActionType);
        }

        public void SwitchToDroidActions()
        {
            RemoveActions(typeof(FocusAction));
            AddActions(new ActionInfo(typeof(CalculateAction)));

            if (LinkedActions.Any(a => a.ActionType == typeof(BoostAction) && a.ActionLinkedType == typeof(FocusAction)))
            {
                RemoveLinkedAction(typeof(BoostAction), typeof(FocusAction));
                AddLinkedAction(new LinkedActionInfo(typeof(BoostAction), typeof(CalculateAction)));
            }

            if (LinkedActions.Any(a => a.ActionType == typeof(BarrelRollAction) && a.ActionLinkedType == typeof(FocusAction)))
            {
                RemoveLinkedAction(typeof(BarrelRollAction), typeof(FocusAction));
                AddLinkedAction(new LinkedActionInfo(typeof(BarrelRollAction), typeof(CalculateAction)));
            }
        }
    }

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
        public List<Type> ActionsThatCanbePreformedwhileStressed;
        private List<AddedAction> AddedActions;

        public ShipActionBar(GenericShip host)
        {
            Host = host;

            AddedActions = new List<AddedAction>();
            LinkedActions = new List<KeyValuePair<Type, GenericAction>>();
        }

        public void Initialize()
        {
            PrintedActions = new List<GenericAction>();
            ActionsThatCanbePreformedwhileStressed = new List<Type>();
            foreach (ActionInfo actionInfo in Host.ShipInfo.ActionIcons.Actions)
            {
                GenericAction action = (GenericAction)Activator.CreateInstance(actionInfo.ActionType);
                action.HostShip = Host;
                action.Color = actionInfo.Color;

                if (actionInfo.Source == null)
                {
                    AddPrintedAction(action);
                }
                else
                {
                    AddGrantedAction(action, actionInfo.Source);
                }
                
            }

            foreach (LinkedActionInfo linkedActionInfo in Host.ShipInfo.ActionIcons.LinkedActions)
            {
                GenericAction linkedAction = (GenericAction)Activator.CreateInstance(linkedActionInfo.ActionLinkedType);
                linkedAction.HostShip = Host;
                linkedAction.Color = linkedActionInfo.LinkedColor;

                AddActionLink(linkedActionInfo.ActionType, linkedAction);
            }
        }

        public void AddPrintedAction(GenericAction action)
        {
            action.IsInActionBar = true;
            PrintedActions.Add(action);
        }

        public void AddActionLink(Type actionType, GenericAction link)
        {
            LinkedActions.Add(new KeyValuePair<Type, GenericAction>(actionType, link));
        }

        public void RemoveActionLink(Type actionType, Type linkedRedAction)
        {
            KeyValuePair<Type, GenericAction> linkToRemove = LinkedActions.FirstOrDefault(n => n.Key == actionType && n.Value.GetType() == linkedRedAction);
            LinkedActions.Remove(linkToRemove);
        }

        public void AddGrantedAction(GenericAction action, GenericUpgrade source)
        {
            action.HostShip = Host;
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
            return AllActions.Any(n => (n.GetType() == type || n.GetType().IsSubclassOf(type)) && n.IsRed == isRed);
        }
    }
}

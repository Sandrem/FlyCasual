using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    public partial class GenericShip
    {

        protected   List<ActionsList.GenericAction> BuiltInActions                  = new List<ActionsList.GenericAction>();
        private     List<ActionsList.GenericAction> AvailableActionsList            = new List<ActionsList.GenericAction>();
        private     List<ActionsList.GenericAction> AvailableFreeActionsList        = new List<ActionsList.GenericAction>();
        private     List<ActionsList.GenericAction> AlreadyExecutedActions          = new List<ActionsList.GenericAction>();
        private     List<ActionsList.GenericAction> AvailableActionEffects          = new List<ActionsList.GenericAction>();
        private     List<ActionsList.GenericAction> AlreadyExecutedActionEffects    = new List<ActionsList.GenericAction>();

        private     List<Tokens.GenericToken> AssignedTokens = new List<Tokens.GenericToken>();

        // EVENTS

        public event EventHandlerShip AfterGenerateAvailableActionsList;
        public event EventHandlerActionBool OnTryAddAvailableAction;

        public event EventHandlerShip AfterGenerateAvailableActionEffectsList;
        public static event EventHandler AfterGenerateAvailableActionEffectsListGlobal;
        public event EventHandlerActionBool OnTryAddAvailableActionEffect;

        public event EventHandlerShip OnActionSubphaseEnd;

        public event EventHandlerShipType AfterTokenIsAssigned;
        public event EventHandlerShipType AfterTokenIsSpent;
        public event EventHandlerShipType AfterTokenIsRemoved;

        // ACTIONS
        public void CallOnActionSubphaseEnd()
        {
            if (OnActionSubphaseEnd != null) OnActionSubphaseEnd(this);
        }

        private void AddBuiltInActions()
        {
            BuiltInActions.Add(new ActionsList.FocusAction());
        }

        public List<ActionsList.GenericAction> GetActionsFromActionBar()
        {
            return BuiltInActions;
        }

        public void GenerateAvailableActionsList()
        {
            AvailableActionsList = new List<ActionsList.GenericAction>();

            foreach (var action in BuiltInActions)
            {
                AddAvailableAction(action);
            }

            if (AfterGenerateAvailableActionsList != null) AfterGenerateAvailableActionsList(this);
        }

        public void GenerateAvailableFreeActionsList(List<ActionsList.GenericAction> freeActions)
        {
            AvailableFreeActionsList = new List<ActionsList.GenericAction>();
            foreach (var action in freeActions)
            {
                AddAvailableFreeAction(action);
            }

            if (AfterGenerateAvailableActionsList != null) AfterGenerateAvailableActionsList(this);
        }

        public bool CanPerformAction(ActionsList.GenericAction action)
        {
            bool result = true;

            if (OnTryAddAvailableAction != null) OnTryAddAvailableAction(action, ref result);

            return result;
        }


        // TODO: move actions list into subphase
        public void AskPerformFreeAction(List<ActionsList.GenericAction> freeActions, Action callBack)
        {
            GenerateAvailableFreeActionsList(freeActions);

            Triggers.RegisterTrigger(
                new Trigger()
                {
                    Name = "Free action",
                    TriggerOwner = Phases.CurrentPhasePlayer,
                    TriggerType = TriggerTypes.OnFreeAction,
                    EventHandler = delegate {
                        Phases.StartTemporarySubPhase("Free action", typeof(SubPhases.FreeActionSubPhase), callBack);
                    }
                }
            );

            Triggers.ResolveTriggers(TriggerTypes.OnFreeAction, Triggers.FinishTrigger);
        }

        public List<ActionsList.GenericAction> GetAvailableActionsList()
        {
            return AvailableActionsList;
        }

        public List<ActionsList.GenericAction> GetAvailableFreeActionsList()
        {
            return AvailableFreeActionsList;
        }

        public void AddAvailableAction(ActionsList.GenericAction action)
        {
            if (CanPerformAction(action))
            {
                AvailableActionsList.Add(action);
            }
        }

        public void AddAvailableFreeAction(ActionsList.GenericAction action)
        {
            if (CanPerformAction(action))
            {
                AvailableFreeActionsList.Add(action);
            }
        }

        public void AddAlreadyExecutedAction(ActionsList.GenericAction action)
        {
            AlreadyExecutedActions.Add(action);
        }

        public void ClearAlreadyExecutedActions()
        {
            AlreadyExecutedActions = new List<ActionsList.GenericAction>();
        }

        public void RemoveAlreadyExecutedAction(System.Type type)
        {
            List<ActionsList.GenericAction> keys = new List<ActionsList.GenericAction>(AlreadyExecutedActions);

            foreach (var executedAction in keys)
            {
                if (executedAction.GetType() == type)
                {
                    AlreadyExecutedActions.Remove(executedAction);
                    return;
                }
            }
        }

        public bool IsAlreadyExecutedAction(System.Type type)
        {
            bool result = false;
            foreach (var executedAction in AlreadyExecutedActions)
            {
                if (executedAction.GetType() == type)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        // ACTION EFFECTS

        public void GenerateAvailableActionEffectsList()
        {
            AvailableActionEffects = new List<ActionsList.GenericAction>(); ;

            foreach (var token in AssignedTokens)
            {
                ActionsList.GenericAction action = token.GetAvailableEffects();
                if (action != null) AddAvailableActionEffect(action);
            }

            if (AfterGenerateAvailableActionEffectsList != null) AfterGenerateAvailableActionEffectsList(this);

            if (AfterGenerateAvailableActionEffectsListGlobal != null) AfterGenerateAvailableActionEffectsListGlobal();

        }

        public void AddAvailableActionEffect(ActionsList.GenericAction action)
        {
            if (CanUseActionEffect(action))
            {
                AvailableActionEffects.Add(action);
            }
        }

        public void AddAlreadyExecutedActionEffect(ActionsList.GenericAction action)
        {
            AlreadyExecutedActionEffects.Add(action);
        }

        public void ClearAlreadyExecutedActionEffects()
        {
            AlreadyExecutedActionEffects = new List<ActionsList.GenericAction>();
        }

        public bool CanUseActionEffect(ActionsList.GenericAction action)
        {
            bool result = true;

            if (!action.IsActionEffectAvailable()) result = false;

            if (IsAlreadyExecuted(action)) result = false;

            if (result)
            {
                if (OnTryAddAvailableActionEffect != null) OnTryAddAvailableActionEffect(action, ref result);
            }

            return result;
        }

        private bool IsAlreadyExecuted(ActionsList.GenericAction action)
        {
            bool result = false;

            foreach (var alreadyExecuedAction in AlreadyExecutedActionEffects)
            {
                if (alreadyExecuedAction.GetType() == action.GetType())
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        public List<ActionsList.GenericAction> GetAvailableActionEffectsList()
        {
            return AvailableActionEffects;
        }

        // TOKENS

        public bool HasToken(System.Type type, char letter = ' ')
        {
            bool result = false;
            if (GetToken(type, letter) != null) result = true;
            return result;
        }

        public Tokens.GenericToken GetToken(System.Type type, char letter = ' ')
        {
            Tokens.GenericToken result = null;

            foreach (var assignedToken in AssignedTokens)
            {
                if (assignedToken.GetType() == type)
                {
                    if (assignedToken.GetType().BaseType == typeof(Tokens.GenericTargetLockToken))
                    {
                        if (((assignedToken as Tokens.GenericTargetLockToken).Letter == letter) || (letter == '*'))
                        {
                            return assignedToken;
                        }
                    }
                    else
                    {
                        return assignedToken;
                    }
                }
            }
            return result;
        }

        public char GetTargetLockLetterPair(GenericShip targetShip)
        {
            char result = ' ';

            Tokens.GenericToken blueToken = GetToken(typeof(Tokens.BlueTargetLockToken), '*');
            if (blueToken != null)
            {
                char foundLetter = (blueToken as Tokens.BlueTargetLockToken).Letter;

                Tokens.GenericToken redToken = targetShip.GetToken(typeof(Tokens.RedTargetLockToken), foundLetter);
                if (redToken != null)
                {
                    return foundLetter;
                }
            }
            return result;
        }

        public void AssignToken(Tokens.GenericToken token, Action callBack, char letter = ' ')
        {
            Tokens.GenericToken assignedToken = GetToken(token.GetType(), letter);

            if (assignedToken != null)
            {
                assignedToken.Count++;
            }
            else                
            {
                AssignedTokens.Add(token);
            }

            if (AfterTokenIsAssigned != null) AfterTokenIsAssigned(this, token.GetType());

            Triggers.ResolveTriggers(TriggerTypes.OnTokenIsAssigned, callBack);
        }

        public void RemoveToken(System.Type type, char letter = ' ', bool recursive = false)
        {
            Tokens.GenericToken assignedToken = GetToken(type, letter);

            if (assignedToken != null)
            {

                if (assignedToken.Count > 1)
                {
                    assignedToken.Count--;
                    if (AfterTokenIsRemoved != null) AfterTokenIsRemoved(this, type);

                    if (recursive)
                    {
                        RemoveToken(type, letter, true);
                    }
                }
                else
                {
                    if (assignedToken.GetType().BaseType == typeof(Tokens.GenericTargetLockToken))
                    {
                        Actions.ReleaseTargetLockLetter((assignedToken as Tokens.GenericTargetLockToken).Letter);
                    }
                    AssignedTokens.Remove(assignedToken);
                    if (AfterTokenIsRemoved != null) AfterTokenIsRemoved(this, type);
                }
            }
        }

        public void SpendToken(System.Type type, char letter = ' ')
        {
            RemoveToken(type, letter);
            if (AfterTokenIsSpent != null) AfterTokenIsSpent(this, type);
        }

        public List<Tokens.GenericToken> GetAssignedTokens()
        {
            return AssignedTokens;
        }

        public void ClearAllTokens()
        {
            List<Tokens.GenericToken> keys = new List<Tokens.GenericToken>(AssignedTokens);

            foreach (var token in keys)
            {
                if (token.Temporary)
                {
                    RemoveToken(token.GetType(), '*', true);
                }
            }
        }

    }

}

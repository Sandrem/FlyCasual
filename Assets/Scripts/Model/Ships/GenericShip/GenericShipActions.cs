using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Ship
{
    public partial class GenericShip
    {

        public      List<ActionsList.GenericAction> BuiltInActions                          = new List<ActionsList.GenericAction>();
        private     List<ActionsList.GenericAction> AvailableActionsList                    = new List<ActionsList.GenericAction>();
        private     List<ActionsList.GenericAction> AvailableFreeActionsList                = new List<ActionsList.GenericAction>();
        private     List<ActionsList.GenericAction> AlreadyExecutedActions                  = new List<ActionsList.GenericAction>();
        private     List<ActionsList.GenericAction> AvailableActionEffects                  = new List<ActionsList.GenericAction>();
        private     List<ActionsList.GenericAction> AvailableOppositeActionEffects          = new List<ActionsList.GenericAction>();
        private     List<ActionsList.GenericAction> AlreadyExecutedActionEffects            = new List<ActionsList.GenericAction>();
        private     List<ActionsList.GenericAction> AlreadyExecutedOppositeActionEffects    = new List<ActionsList.GenericAction>();

        private     List<Tokens.GenericToken> AssignedTokens = new List<Tokens.GenericToken>();

        // EVENTS

        public event EventHandlerShip AfterGenerateAvailableActionsList;
        public event EventHandlerActionBool OnTryAddAvailableAction;

        public event EventHandlerShip AfterGenerateAvailableActionEffectsList;
        public static event EventHandler AfterGenerateAvailableActionEffectsListGlobal;
        public event EventHandlerActionBool OnTryAddAvailableActionEffect;

        public event EventHandlerShip AfterGenerateAvailableOppositeActionEffectsList;
        public static event EventHandler AfterGenerateAvailableOppositeActionEffectsListGlobal;
        public event EventHandlerActionBool OnTryAddAvailableOppositeActionEffect;

        public event EventHandlerShip OnActionDecisionSubphaseEnd;
        public event EventHandlerAction OnActionIsPerformed;

        public event EventHandlerShipType OnTokenIsAssigned;
        public static event EventHandlerShipType OnTokenIsAssignedGlobal;
        public event EventHandlerShipType OnTokenIsSpent;
        public static event EventHandlerShipType OnTokenIsSpentGlobal;
        public event EventHandlerShipType AfterTokenIsRemoved;

        // ACTIONS
        public void CallOnActionDecisionSubphaseEnd()
        {
            if (OnActionDecisionSubphaseEnd != null) OnActionDecisionSubphaseEnd(this);
        }

        public void CallActionIsTaken(ActionsList.GenericAction action)
        {
            if (OnActionIsPerformed != null) OnActionIsPerformed(action);
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
            bool result = action.IsActionAvailable();

            if (OnTryAddAvailableAction != null) OnTryAddAvailableAction(action, ref result);

            return result;
        }

        public bool CanPerformActionsWhileStressed { get; protected set; }


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
                        Phases.StartTemporarySubPhase
                        (
                            "Free action decision",
                            typeof(SubPhases.FreeActionDecisonSubPhase),
                            delegate
                            {
                                Phases.FinishSubPhase(typeof(SubPhases.FreeActionDecisonSubPhase));
                                callBack();
                            }
                        );
                    }
                }
            );

            Triggers.ResolveTriggers(TriggerTypes.OnFreeAction, Triggers.FinishTrigger);
        }

        public List<ActionsList.GenericAction> GetAvailableActionsList()
        {
            return AvailableActionsList;
        }

        public List<ActionsList.GenericAction> GetAvailablePrintedActionsList()
        {
            return BuiltInActions;
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

        public void RemoveAlreadyExecutedActionEffect(ActionsList.GenericAction action)
        {
            AlreadyExecutedActionEffects.RemoveAll(a => a.GetType() == action.GetType());
        }

        public void ClearAlreadyExecutedActionEffects()
        {
            AlreadyExecutedActionEffects = new List<ActionsList.GenericAction>();
        }

        public bool CanUseActionEffect(ActionsList.GenericAction action)
        {
            bool result = true;

            if (!action.IsActionEffectAvailable()) result = false;

            if (IsActionEffectAlreadyExecuted(action)) result = false;

            if (result)
            {
                if (OnTryAddAvailableActionEffect != null) OnTryAddAvailableActionEffect(action, ref result);
            }

            return result;
        }

        private bool IsActionEffectAlreadyExecuted(ActionsList.GenericAction action)
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

        // OPPOSITE ACTION EFFECTS

        public void GenerateAvailableOppositeActionEffectsList()
        {
            AvailableOppositeActionEffects = new List<ActionsList.GenericAction>();

            if (AfterGenerateAvailableOppositeActionEffectsList != null) AfterGenerateAvailableOppositeActionEffectsList(this);

            if (AfterGenerateAvailableOppositeActionEffectsListGlobal != null) AfterGenerateAvailableOppositeActionEffectsListGlobal();
        }

        public void AddAvailableOppositeActionEffect(ActionsList.GenericAction action)
        {
            if (CanUseOppositeActionEffect(action))
            {
                AvailableOppositeActionEffects.Add(action);
            }
        }

        public void AddAlreadyExecutedOppositeActionEffect(ActionsList.GenericAction action)
        {
            AlreadyExecutedOppositeActionEffects.Add(action);
        }

        public void RemoveAlreadyExecutedOppositeActionEffect(ActionsList.GenericAction action)
        {
            AlreadyExecutedOppositeActionEffects.RemoveAll(a => a.GetType() == action.GetType());
        }

        public void ClearAlreadyExecutedOppositeActionEffects()
        {
            AlreadyExecutedOppositeActionEffects = new List<ActionsList.GenericAction>();
        }

        public bool CanUseOppositeActionEffect(ActionsList.GenericAction action)
        {
            bool result = true;

            if (!action.IsActionEffectAvailable()) result = false;

            if (IsOppositeActionEffectAlreadyExecuted(action)) result = false;

            if (result)
            {
                if (OnTryAddAvailableOppositeActionEffect != null) OnTryAddAvailableOppositeActionEffect(action, ref result);
            }

            return result;
        }

        private bool IsOppositeActionEffectAlreadyExecuted(ActionsList.GenericAction action)
        {
            bool result = false;

            foreach (var alreadyExecuedOppositeAction in AlreadyExecutedOppositeActionEffects)
            {
                if (alreadyExecuedOppositeAction.GetType() == action.GetType())
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        public List<ActionsList.GenericAction> GetAvailableOppositeActionEffectsList()
        {
            return AvailableOppositeActionEffects;
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

            if (OnTokenIsAssigned != null) OnTokenIsAssigned(this, token.GetType());

            if (OnTokenIsAssignedGlobal != null) OnTokenIsAssignedGlobal(this, token.GetType());

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
                    AssignedTokens.Remove(assignedToken);
                    if (AfterTokenIsRemoved != null) AfterTokenIsRemoved(this, type);

                    if (assignedToken.GetType().BaseType == typeof(Tokens.GenericTargetLockToken))
                    {
                        GenericShip otherTokenOwner = (assignedToken as Tokens.GenericTargetLockToken).OtherTokenOwner;
                        Actions.ReleaseTargetLockLetter((assignedToken as Tokens.GenericTargetLockToken).Letter);
                        System.Type oppositeType = (assignedToken.GetType() == typeof(Tokens.BlueTargetLockToken)) ? typeof(Tokens.RedTargetLockToken) : typeof(Tokens.BlueTargetLockToken);

                        if (otherTokenOwner.HasToken(oppositeType, letter))
                        {
                            otherTokenOwner.RemoveToken(oppositeType, letter);
                        }
                    }
                }
            }
        }

        public void SpendToken(System.Type type, Action callBack, char letter = ' ')
        {
            RemoveToken(type, letter);

            if (OnTokenIsSpent != null) OnTokenIsSpent(this, type);

            if (OnTokenIsSpentGlobal != null) OnTokenIsSpentGlobal(this, type);

            Triggers.ResolveTriggers(TriggerTypes.OnTokenIsSpent, callBack);
        }

        public List<Tokens.GenericToken> GetAssignedTokens()
        {
            return AssignedTokens;
        }

        public EventHandlerTokenBool BeforeRemovingTokenInEndPhase;

        private bool ShoulRemoveTokenInEndPhase(Tokens.GenericToken token)
        {
            var remove = token.Temporary;
            if (BeforeRemovingTokenInEndPhase != null) BeforeRemovingTokenInEndPhase(token, ref remove);
            return remove;
        }


        public void ClearAllTokens()
        {
            List<Tokens.GenericToken> keys = new List<Tokens.GenericToken>(AssignedTokens);

            foreach (var token in keys)
            {
                if (ShoulRemoveTokenInEndPhase(token))
                {
                    RemoveToken(token.GetType(), '*', true);
                }
            }
        }

    }

}

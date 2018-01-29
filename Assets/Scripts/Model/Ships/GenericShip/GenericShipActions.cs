using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using SubPhases;
using Tokens;

namespace Ship
{
    public partial class GenericShip
    {

        public      List<ActionsList.GenericAction> PrintedActions                          = new List<ActionsList.GenericAction>();
        private     List<ActionsList.GenericAction> AvailableActionsList                    = new List<ActionsList.GenericAction>();
        private     List<ActionsList.GenericAction> AvailableFreeActionsList                = new List<ActionsList.GenericAction>();
        private     List<ActionsList.GenericAction> AlreadyExecutedActions                  = new List<ActionsList.GenericAction>();
        private     List<ActionsList.GenericAction> AvailableActionEffects                  = new List<ActionsList.GenericAction>();
        private     List<ActionsList.GenericAction> AvailableOppositeActionEffects          = new List<ActionsList.GenericAction>();
        private     List<ActionsList.GenericAction> AlreadyExecutedActionEffects            = new List<ActionsList.GenericAction>();
        private     List<ActionsList.GenericAction> AlreadyExecutedOppositeActionEffects    = new List<ActionsList.GenericAction>();

        public     List<GenericToken> AssignedTokens = new List<GenericToken>();

        public GenericToken TokenToAssign;

        // EVENTS
        public event EventHandlerShip OnActivateShip;

        public event EventHandlerShip AfterGenerateAvailableActionsList;
        public event EventHandlerActionBool OnTryAddAvailableAction;

        public event EventHandlerShip AfterGenerateAvailableActionEffectsList;
        public static event EventHandler AfterGenerateAvailableActionEffectsListGlobal;
        public event EventHandlerActionBool OnTryAddAvailableActionEffect;
        public static event EventHandlerShipActionBool OnTryAddAvailableActionEffectGlobal;

        public event EventHandlerShip AfterGenerateAvailableOppositeActionEffectsList;
        public static event EventHandler AfterGenerateAvailableOppositeActionEffectsListGlobal;
        public event EventHandlerActionBool OnTryAddAvailableOppositeActionEffect;

        public event EventHandlerShip OnActionDecisionSubphaseEnd;
        public event EventHandlerAction OnActionIsPerformed;

        public event EventHandlerShipType OnTokenIsAssigned;
        public event EventHandlerShipType BeforeTokenIsAssigned;
        public static event EventHandlerShipType BeforeTokenIsAssignedGlobal;
        public static event EventHandlerShipType OnTokenIsAssignedGlobal;
        public event EventHandlerShipType OnTokenIsSpent;
        public static event EventHandlerShipType OnTokenIsSpentGlobal;
        public event EventHandlerShipType OnTokenIsRemoved;
        public event EventHandlerShipType OnConditionIsRemoved;

        public event EventHandlerShip OnCoordinateTargetIsSelected;

        public event EventHandlerShip OnRerollIsConfirmed;

        // ACTIONS

        public void CallActivateShip(Action callBack)
        {
            if (OnActivateShip != null) OnActivateShip(this);

            Triggers.ResolveTriggers(TriggerTypes.OnActivateShip, callBack);
        }

        public void CallOnActionDecisionSubphaseEnd(Action callback)
        {
            if (OnActionDecisionSubphaseEnd != null) OnActionDecisionSubphaseEnd(this);

            Triggers.ResolveTriggers(TriggerTypes.OnActionDecisionSubPhaseEnd, callback);
        }

        public void CallActionIsTaken(ActionsList.GenericAction action, Action callBack)
        {
            if (OnActionIsPerformed != null) OnActionIsPerformed(action);

            Triggers.ResolveTriggers(TriggerTypes.OnActionIsPerformed, callBack);
        }

        public List<ActionsList.GenericAction> GetActionsFromActionBar()
        {
            return PrintedActions;
        }

        public void GenerateAvailableActionsList()
        {
            AvailableActionsList = new List<ActionsList.GenericAction>();

            foreach (var action in PrintedActions)
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

        // TODO: move actions list into subphase
        public void AskPerformFreeAction(List<ActionsList.GenericAction> freeActions, Action callback)
        {
            GenerateAvailableFreeActionsList(freeActions);

            Triggers.RegisterTrigger(
                new Trigger()
                {
                    Name = "Free action",
                    TriggerOwner = Phases.CurrentPhasePlayer,
                    TriggerType = TriggerTypes.OnFreeAction,
                    EventHandler = delegate {
                        Phases.StartTemporarySubPhaseOld
                        (
                            "Free action decision",
                            typeof(SubPhases.FreeActionDecisonSubPhase),
                            delegate { Actions.FinishAction(delegate { FinishFreeActionDecision(callback); }); }
                        );
                    }
                }
            );

            Triggers.ResolveTriggers(TriggerTypes.OnFreeAction, Triggers.FinishTrigger);
        }

        private void FinishFreeActionDecision(Action callback)
        {
            Phases.FinishSubPhase(typeof(SubPhases.FreeActionDecisonSubPhase));
            callback();
        }

        public List<ActionsList.GenericAction> GetAvailableActionsList()
        {
            return AvailableActionsList;
        }

        public List<ActionsList.GenericAction> GetAvailablePrintedActionsList()
        {
            return PrintedActions;
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

                if (OnTryAddAvailableActionEffectGlobal != null) OnTryAddAvailableActionEffectGlobal(this, action, ref result);
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

        public int TokenCount(Type type)
        {
            return GetAllTokens().Count(n => n.GetType() == type);
        }

        public GenericToken GetToken(System.Type type, char letter = ' ')
        {
            GenericToken result = null;

            foreach (var assignedToken in AssignedTokens)
            {
                if (assignedToken.GetType() == type)
                {
                    if (assignedToken.GetType().BaseType == typeof(GenericTargetLockToken))
                    {
                        if (((assignedToken as GenericTargetLockToken).Letter == letter) || (letter == '*'))
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

        public GenericTargetLockToken GetTargetLockToken(char letter)
        {
            return (GenericTargetLockToken) AssignedTokens.Find(n => n.GetType().BaseType == typeof(GenericTargetLockToken) && (n as GenericTargetLockToken).Letter == letter);
        }

        public char GetTargetLockLetterPair(GenericShip targetShip)
        {
            char result = ' ';

            GenericToken blueToken = GetToken(typeof(BlueTargetLockToken), '*');
            if (blueToken != null)
            {
                char foundLetter = (blueToken as BlueTargetLockToken).Letter;

                GenericToken redToken = targetShip.GetToken(typeof(RedTargetLockToken), foundLetter);
                if (redToken != null)
                {
                    return foundLetter;
                }
            }
            return result;
        }

        public void AssignToken(GenericToken token, Action callBack, char letter = ' ')
        {
            TokenToAssign = token;

            if (BeforeTokenIsAssigned != null) BeforeTokenIsAssigned(this, token.GetType());
            if (BeforeTokenIsAssignedGlobal != null) BeforeTokenIsAssignedGlobal(this, token.GetType());

            Triggers.ResolveTriggers(TriggerTypes.OnBeforeTokenIsAssigned, delegate { FinalizeAssignToken(callBack, letter); });
        }

        private void FinalizeAssignToken(Action callBack, char letter = ' ')
        {
            if (TokenToAssign == null)
            {
                callBack();
                return;
            }

            AssignedTokens.Add(TokenToAssign);

            if (OnTokenIsAssigned != null) OnTokenIsAssigned(this, TokenToAssign.GetType());

            if (OnTokenIsAssignedGlobal != null) OnTokenIsAssignedGlobal(this, TokenToAssign.GetType());

            TokenToAssign = null;

            Triggers.ResolveTriggers(TriggerTypes.OnTokenIsAssigned, callBack);
        }

        public void RemoveCondition(GenericToken token)
        {
            if (AssignedTokens.Remove(token))
            {
                if (OnConditionIsRemoved != null) OnConditionIsRemoved(this, token.GetType());
            }
        }

        public void RemoveCondition(System.Type type)
        {
            GenericToken assignedCondition = GetToken(type);
            RemoveCondition(assignedCondition);
        }

        public void RemoveToken(System.Type type, Action callback, char letter = ' ')
        {
            GenericToken assignedToken = GetToken(type, letter);

            if (assignedToken == null)
            {
                callback();
            }
            else
            {
                RemoveToken(assignedToken, callback, letter);
            }
        }

        public void RemoveToken(GenericToken tokenToRemove, Action callback, char letter = ' ')
        {
            AssignedTokens.Remove(tokenToRemove);

            if (tokenToRemove.GetType().BaseType == typeof(GenericTargetLockToken))
            {
                GenericShip otherTokenOwner = (tokenToRemove as GenericTargetLockToken).OtherTokenOwner;
                Actions.ReleaseTargetLockLetter((tokenToRemove as GenericTargetLockToken).Letter);
                System.Type oppositeType = (tokenToRemove.GetType() == typeof(BlueTargetLockToken)) ? typeof(RedTargetLockToken) : typeof(BlueTargetLockToken);

                GenericToken otherTargetLockToken = otherTokenOwner.GetToken(oppositeType, letter);
                if (otherTargetLockToken != null)
                {
                    otherTokenOwner.AssignedTokens.Remove(otherTargetLockToken);
                    if (OnTokenIsRemoved != null) OnTokenIsRemoved(this, otherTargetLockToken.GetType());
                }
            }

            if (OnTokenIsRemoved != null) OnTokenIsRemoved(this, tokenToRemove.GetType());

            Triggers.ResolveTriggers(TriggerTypes.OnTokenIsRemoved, callback);
        }

        public void RemoveAllTokensByType(System.Type tokenType, Action callback)
        {
            GenericToken tokenToRemove = GetToken(tokenType);
            if (tokenToRemove != null)
            {
                RemoveToken(
                    tokenType,
                    delegate { RemoveAllTokensByType(tokenType, callback); }
                );
            }
            else
            {
                callback();
            }
        }

        public void AcquireTargetLock(Action callback)
        {
            AcquireTargetLockSubPhase selectTargetLockSubPhase = (AcquireTargetLockSubPhase)Phases.StartTemporarySubPhaseNew(
                "Select target for Target Lock",
                typeof(AcquireTargetLockSubPhase),
                delegate {
                    UI.HideSkipButton();
                    Phases.FinishSubPhase(typeof(AcquireTargetLockSubPhase));
                    callback();
                });

            selectTargetLockSubPhase.RequiredPlayer = Owner.PlayerNo;
            selectTargetLockSubPhase.Start();
        }

        public void SpendToken(System.Type type, Action callback, char letter = ' ')
        {
            RemoveToken(
                type,
                delegate { FinishSpendToken(type, callback); },
                letter
            );
        }

        private void FinishSpendToken(System.Type type, Action callback)
        {
            if (OnTokenIsSpent != null) OnTokenIsSpent(this, type);

            if (OnTokenIsSpentGlobal != null) OnTokenIsSpentGlobal(this, type);

            Triggers.ResolveTriggers(TriggerTypes.OnTokenIsSpent, callback);
        }

        public List<GenericToken> GetAllTokens()
        {
            return AssignedTokens;
        }

        public EventHandlerTokenBool BeforeRemovingTokenInEndPhase;

        public bool ShouldRemoveTokenInEndPhase(GenericToken token)
        {
            var remove = token.Temporary;
            if (BeforeRemovingTokenInEndPhase != null) BeforeRemovingTokenInEndPhase(token, ref remove);
            return remove;
        }

        // Coordinate

        public void CallCoordinateTargetIsSelected(GenericShip targetShip, Action callback)
        {
            if (OnCoordinateTargetIsSelected != null) OnCoordinateTargetIsSelected(targetShip);

            Triggers.ResolveTriggers(TriggerTypes.OnCoordinateTargetIsSelected, callback);
        }

        // Reroll is confirmed

        public void CallRerollIsConfirmed(Action callback)
        {
            if (OnRerollIsConfirmed != null) OnRerollIsConfirmed(this);

            Triggers.ResolveTriggers(TriggerTypes.OnRerollIsConfirmed, callback);
        }

    }

}

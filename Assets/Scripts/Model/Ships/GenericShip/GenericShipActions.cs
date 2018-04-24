using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using SubPhases;
using Tokens;
using ActionsList;

namespace Ship
{
    public partial class GenericShip
    {

        public      List<GenericAction> PrintedActions                          = new List<GenericAction>();
        private     List<GenericAction> AvailableActionsList                    = new List<GenericAction>();
        private     List<GenericAction> AvailableFreeActionsList                = new List<GenericAction>();
        private     List<GenericAction> AlreadyExecutedActions                  = new List<GenericAction>();
        private     List<GenericAction> AvailableActionEffects                  = new List<GenericAction>();
        private     List<GenericAction> AvailableOppositeActionEffects          = new List<GenericAction>();
        private     List<GenericAction> AvailableCompareResultsEffects          = new List<GenericAction>();
        private     List<GenericAction> AlreadyExecutedActionEffects            = new List<GenericAction>();
        private     List<GenericAction> AlreadyExecutedOppositeActionEffects    = new List<GenericAction>();
        private     List<GenericAction> AlreadyExecutedCompareResultsEffects    = new List<GenericAction>();

        // EVENTS
        public event EventHandlerShip OnMovementActivation;

        public event EventHandlerShip AfterGenerateAvailableActionsList;
        public event EventHandlerActionBool OnTryAddAvailableAction;
        public static event EventHandlerShipActionBool OnTryAddAvailableActionGlobal;

        public event EventHandlerShip AfterGenerateAvailableActionEffectsList;
        public static event EventHandler AfterGenerateAvailableActionEffectsListGlobal;
        public event EventHandlerActionBool OnTryAddAvailableActionEffect;
        public static event EventHandlerShipActionBool OnTryAddAvailableActionEffectGlobal;

        public event EventHandlerShip AfterGenerateAvailableOppositeActionEffectsList;
        public static event EventHandler AfterGenerateAvailableOppositeActionEffectsListGlobal;
        public event EventHandlerActionBool OnTryAddAvailableOppositeActionEffect;

        public event EventHandlerShip AfterGenerateAvailableCompareResultsEffectsList;
        public static event EventHandler AfterGenerateAvailableCompareResultsEffectsListGlobal;
        public event EventHandlerActionBool OnTryAddAvailableCompareResultsEffect;

        public event EventHandlerShip OnActionDecisionSubphaseEnd;
        public event EventHandlerAction OnActionIsPerformed;

        public event EventHandlerShipType OnTokenIsAssigned;
        public event EventHandlerShipType BeforeTokenIsAssigned;
        public static event EventHandlerShipType BeforeTokenIsAssignedGlobal;
        public static event EventHandlerShipType OnTokenIsAssignedGlobal;
        public event EventHandlerShipType OnTokenIsSpent;
        public static event EventHandlerShipType OnTokenIsSpentGlobal;
        public event EventHandlerShipType OnTokenIsRemoved;
        public static event EventHandlerShipType OnTokenIsRemovedGlobal;

        public event EventHandlerShipType OnConditionIsAssigned;
        public event EventHandlerShipType OnConditionIsRemoved;

        public event EventHandlerShip OnTargetLockIsAcquired;

        public event EventHandlerShip OnCoordinateTargetIsSelected;

        public event EventHandlerShip OnRerollIsConfirmed;

        public EventHandlerTokenBool BeforeRemovingTokenInEndPhase;

        // ACTIONS

        public void CallMovementActivation(Action callBack)
        {
            if (OnMovementActivation != null) OnMovementActivation(this);

            Triggers.ResolveTriggers(TriggerTypes.OnMovementActivation, callBack);
        }

        public void CallOnActionDecisionSubphaseEnd(Action callback)
        {
            if (OnActionDecisionSubphaseEnd != null) OnActionDecisionSubphaseEnd(this);

            Triggers.ResolveTriggers(TriggerTypes.OnActionDecisionSubPhaseEnd, callback);
        }

        public void CallActionIsTaken(GenericAction action, Action callBack)
        {
            if (OnActionIsPerformed != null) OnActionIsPerformed(action);

            Triggers.ResolveTriggers(TriggerTypes.OnActionIsPerformed, callBack);
        }

        public List<GenericAction> GetActionsFromActionBar()
        {
            return PrintedActions;
        }

        public void GenerateAvailableActionsList()
        {
            AvailableActionsList = new List<GenericAction>();

            foreach (var action in PrintedActions)
            {
                AddAvailableAction(action);
            }

            if (AfterGenerateAvailableActionsList != null) AfterGenerateAvailableActionsList(this);
        }

        public void GenerateAvailableFreeActionsList(List<GenericAction> freeActions)
        {
            AvailableFreeActionsList = new List<GenericAction>();
            foreach (var action in freeActions)
            {
                AddAvailableFreeAction(action);
            }

            if (AfterGenerateAvailableActionsList != null) AfterGenerateAvailableActionsList(this);
        }

        public bool CanPerformAction(GenericAction action)
        {
            bool result = action.IsActionAvailable();

            if (OnTryAddAvailableAction != null) OnTryAddAvailableAction(action, ref result);

            if (OnTryAddAvailableActionGlobal != null) OnTryAddAvailableActionGlobal(this, action, ref result);

            return result;
        }

        public bool CanPerformFreeAction(GenericAction action)
        {
            bool result = action.IsActionAvailable() && action.CanBePerformedAsAFreeAction;

            if (OnTryAddAvailableAction != null) OnTryAddAvailableAction(action, ref result);

            if (OnTryAddAvailableActionGlobal != null) OnTryAddAvailableActionGlobal(this, action, ref result);

            return result;
        }

        // TODO: move actions list into subphase
        public void AskPerformFreeAction(List<GenericAction> freeActions, Action callback)
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
                            typeof(FreeActionDecisonSubPhase),
                            delegate { Actions.FinishAction(delegate { FinishFreeActionDecision(callback); }); }
                        );
                    }
                }
            );

            Triggers.ResolveTriggers(TriggerTypes.OnFreeAction, Triggers.FinishTrigger);
        }

        private void FinishFreeActionDecision(Action callback)
        {
            Phases.FinishSubPhase(typeof(FreeActionDecisonSubPhase));
            callback();
        }

        public List<GenericAction> GetAvailableActionsList()
        {
            return AvailableActionsList;
        }

        public List<GenericAction> GetAvailablePrintedActionsList()
        {
            return PrintedActions;
        }

        public List<GenericAction> GetAvailableFreeActionsList()
        {
            return AvailableFreeActionsList;
        }

        public void AddAvailableAction(GenericAction action)
        {
            if (CanPerformAction(action))
            {
                AvailableActionsList.Add(action);
            }
        }

        public void AddAvailableFreeAction(GenericAction action)
        {
            if (CanPerformFreeAction(action))
            {
                AvailableFreeActionsList.Add(action);
            }
        }

        public void AddAlreadyExecutedAction(GenericAction action)
        {
            AlreadyExecutedActions.Add(action);
        }

        public void ClearAlreadyExecutedActions()
        {
            AlreadyExecutedActions = new List<GenericAction>();
        }

        public void RemoveAlreadyExecutedAction(System.Type type)
        {
            List<GenericAction> keys = new List<GenericAction>(AlreadyExecutedActions);

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
            AvailableActionEffects = new List<GenericAction>(); ;

            foreach (var token in Tokens.GetAllTokens())
            {
                GenericAction action = token.GetAvailableEffects();
                if (action != null) AddAvailableActionEffect(action);
            }

            if (AfterGenerateAvailableActionEffectsList != null) AfterGenerateAvailableActionEffectsList(this);

            if (AfterGenerateAvailableActionEffectsListGlobal != null) AfterGenerateAvailableActionEffectsListGlobal();
        }

        public void AddAvailableActionEffect(GenericAction action)
        {
            if (NotAlreadyAddedSameActionEffect(action) && CanUseActionEffect(action))
            {
                AvailableActionEffects.Add(action);
            }
        }

        private bool NotAlreadyAddedSameActionEffect(GenericAction action)
        {
            // Return true if AvailableActionEffects doesn't contain action of the same type
            return AvailableActionEffects.FirstOrDefault(n => n.GetType() == action.GetType()) == null;
        }

        public void AddAlreadyExecutedActionEffect(GenericAction action)
        {
            AlreadyExecutedActionEffects.Add(action);
        }

        public void RemoveAlreadyExecutedActionEffect(GenericAction action)
        {
            AlreadyExecutedActionEffects.RemoveAll(a => a.GetType() == action.GetType());
        }

        public void ClearAlreadyExecutedActionEffects()
        {
            AlreadyExecutedActionEffects = new List<GenericAction>();
        }

        public bool CanUseActionEffect(GenericAction action)
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

        private bool IsActionEffectAlreadyExecuted(GenericAction action)
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

        public List<GenericAction> GetAvailableActionEffectsList()
        {
            return AvailableActionEffects;
        }

        // COMPARE DICE RESULTS ACTION EFFECTS

        public void GenerateAvailableCompareResultsEffectsList()
        {
            AvailableCompareResultsEffects = new List<GenericAction>();

            if (AfterGenerateAvailableCompareResultsEffectsList != null) AfterGenerateAvailableCompareResultsEffectsList(this);
        }

        public void AddAvailableCompareResultsEffect(GenericAction action)
        {
            if (CanUseCompareResultsEffect(action))
            {
                AvailableCompareResultsEffects.Add(action);
            }
        }

        public void AddAlreadyExecutedCompareResultsEffect(GenericAction action)
        {
            AlreadyExecutedCompareResultsEffects.Add(action);
        }

        public void RemoveAlreadyExecutedCompareResultsEffect(GenericAction action)
        {
            AlreadyExecutedCompareResultsEffects.RemoveAll(a => a.GetType() == action.GetType());
        }

        public bool CanUseCompareResultsEffect(GenericAction action)
        {
            bool result = true;

            if (!action.IsActionEffectAvailable()) result = false;

            if (IsCompareResultsEffectAlreadyExecuted(action)) result = false;

            if (result)
            {
                if (OnTryAddAvailableCompareResultsEffect != null) OnTryAddAvailableCompareResultsEffect(action, ref result);
            }

            return result;
        }

        private bool IsCompareResultsEffectAlreadyExecuted(GenericAction action)
        {
            bool result = false;

            foreach (var alreadyExecuedCompareResultsEffect in AlreadyExecutedCompareResultsEffects)
            {
                if (alreadyExecuedCompareResultsEffect.GetType() == action.GetType())
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        public List<GenericAction> GetAvailableCompareResultsEffectsList()
        {
            return AvailableCompareResultsEffects;
        }

        // OPPOSITE ACTION EFFECTS

        public void GenerateAvailableOppositeActionEffectsList()
        {
            AvailableOppositeActionEffects = new List<GenericAction>();

            if (AfterGenerateAvailableOppositeActionEffectsList != null) AfterGenerateAvailableOppositeActionEffectsList(this);

            if (AfterGenerateAvailableOppositeActionEffectsListGlobal != null) AfterGenerateAvailableOppositeActionEffectsListGlobal();
        }

        public void AddAvailableOppositeActionEffect(GenericAction action)
        {
            if (CanUseOppositeActionEffect(action))
            {
                AvailableOppositeActionEffects.Add(action);
            }
        }

        public void AddAlreadyExecutedOppositeActionEffect(GenericAction action)
        {
            AlreadyExecutedOppositeActionEffects.Add(action);
        }

        public void RemoveAlreadyExecutedOppositeActionEffect(GenericAction action)
        {
            AlreadyExecutedOppositeActionEffects.RemoveAll(a => a.GetType() == action.GetType());
        }

        public void ClearAlreadyExecutedOppositeActionEffects()
        {
            AlreadyExecutedOppositeActionEffects = new List<GenericAction>();
        }

        public bool CanUseOppositeActionEffect(GenericAction action)
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

        private bool IsOppositeActionEffectAlreadyExecuted(GenericAction action)
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

        public List<GenericAction> GetAvailableOppositeActionEffectsList()
        {
            return AvailableOppositeActionEffects;
        }

        // TOKENS

        public void CallBeforeAssignToken(GenericToken token, Action callback)
        {
            if (BeforeTokenIsAssigned != null) BeforeTokenIsAssigned(this, token.GetType());
            if (BeforeTokenIsAssignedGlobal != null) BeforeTokenIsAssignedGlobal(this, token.GetType());

            Triggers.ResolveTriggers(TriggerTypes.OnBeforeTokenIsAssigned, callback);
        }

        public void CallOnTokenIsAssigned(GenericToken token, Action callback)
        {
            if (OnTokenIsAssigned != null) OnTokenIsAssigned(this, token.GetType());

            if (OnTokenIsAssignedGlobal != null) OnTokenIsAssignedGlobal(this, token.GetType());

            Tokens.TokenToAssign = null;

            Triggers.ResolveTriggers(TriggerTypes.OnTokenIsAssigned, callback);
        }

        public void CallOnConditionIsAssigned(System.Type tokenType)
        {
            if (OnConditionIsAssigned != null) OnConditionIsAssigned(this, tokenType);
        }

        public void CallOnConditionIsRemoved(System.Type tokenType)
        {
            if (OnConditionIsRemoved != null) OnConditionIsRemoved(this, tokenType);
        }

        public void CallOnRemoveTokenEvent(System.Type tokenType)
        {
            if (OnTokenIsRemoved != null) OnTokenIsRemoved(this, tokenType);

            if (OnTokenIsRemovedGlobal != null) OnTokenIsRemovedGlobal(this, tokenType);
        }

        public void CallOnTargetLockIsAcquiredEvent(GenericShip target, Action callback)
        {
            if (OnTargetLockIsAcquired != null) OnTargetLockIsAcquired(target);

            Triggers.ResolveTriggers(TriggerTypes.OnTargetLockIsAcquired, callback);
        }

        public void ChooseTargetToAcquireTargetLock(Action callback, string abilityName, string imageUrl)
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
            selectTargetLockSubPhase.AbilityName = abilityName;
            selectTargetLockSubPhase.ImageUrl = imageUrl;
            selectTargetLockSubPhase.Start();
        }

        public void CallFinishSpendToken(Type type, Action callback)
        {
            if (OnTokenIsSpent != null) OnTokenIsSpent(this, type);

            if (OnTokenIsSpentGlobal != null) OnTokenIsSpentGlobal(this, type);

            Triggers.ResolveTriggers(TriggerTypes.OnTokenIsSpent, callback);
        }

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

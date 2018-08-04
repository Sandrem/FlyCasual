using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using SubPhases;
using Tokens;
using ActionsList;
using GameModes;

namespace Ship
{
    public partial class GenericShip
    {

        private     List<GenericAction> AvailableActionsList                            = new List<GenericAction>();
        private     List<GenericAction> AvailableFreeActionsList                        = new List<GenericAction>();
        private     List<GenericAction> AlreadyExecutedActions                          = new List<GenericAction>();
        private     List<GenericAction> AvailableDiceModifications                      = new List<GenericAction>();
        private     List<GenericAction> AvailableDiceModificationsOpposite              = new List<GenericAction>();
        private     List<GenericAction> AvailableDiceModificationsCompareResults        = new List<GenericAction>();
        private     List<GenericAction> AlreadyExecuteDiceModifications                 = new List<GenericAction>();
        private     List<GenericAction> AlreadyExecutedDiceModificationsOpposite        = new List<GenericAction>();
        private     List<GenericAction> AlreadyExecutedDiceModificationsCompareResults  = new List<GenericAction>();

        public List<GenericAction> PlannedLinkedActions;

        // EVENTS
        public event EventHandlerShip OnMovementActivation;

        public event EventHandlerShip OnGenerateActions;
        public event EventHandlerActionBool OnTryAddAction;
        public static event EventHandlerShipActionBool OnTryAddActionGlobal;

        public event EventHandlerShip OnGenerateDiceModifications;
        public static event EventHandlerShip OnGenerateDiceModificationsGlobal;
        public event EventHandlerShipActionBool OnTryAddAvailableDiceModification;
        public static event EventHandlerShipActionBool OnTryAddAvailableDiceModificationGlobal;

        public event EventHandlerShip OnGenerateDiceModificationsOpposite;
        public static event EventHandlerShip OnGenerateDiceModificationsOppositeGlobal;
        public event EventHandlerShipActionBool OnTryAddDiceModificationOpposite;

        public event EventHandlerShip OnGenerateDiceModificationsCompareResults;
        public static event EventHandlerShip OnGenerateDiceModificationsCompareResultsGlobal;
        public event EventHandlerActionBool OnTryAddDiceModificationCompareResults;

        public event EventHandlerShip OnActionDecisionSubphaseEnd;
        public event EventHandlerAction BeforeFreeActionIsPerformed;
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
        public event EventHandlerShip OnJamTargetIsSelected;        

        public event EventHandlerShip OnRerollIsConfirmed;

        public EventHandlerTokenBool BeforeRemovingTokenInEndPhase;

        public event EventHandler OnDecloak;

        // ACTIONS

        public void GenerateAvailableActionsList()
        {
            AvailableActionsList = new List<GenericAction>();

            foreach (var action in ActionBar.AllActions)
            {
                AddAvailableAction(action);
            }

            if (OnGenerateActions != null) OnGenerateActions(this);
        }

        public List<GenericAction> GetAvailableActions()
        {
            GenerateAvailableActionsList();
            return AvailableActionsList;
        }

        public List<GenericAction> GetAvailableFreeActions()
        {
            return AvailableFreeActionsList;
        }

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
        
        public void CallBeforeFreeActionIsPerformed(GenericAction action, Action callBack)
        {
            if (BeforeFreeActionIsPerformed != null) BeforeFreeActionIsPerformed(action);

            Triggers.ResolveTriggers(TriggerTypes.BeforeFreeActionIsPerformed, callBack);
        }

        public void GenerateAvailableFreeActionsList(List<GenericAction> freeActions)
        {
            AvailableFreeActionsList = new List<GenericAction>();
            foreach (var action in freeActions)
            {
                AddAvailableFreeAction(action);
            }

            if (OnGenerateActions != null) OnGenerateActions(this);
        }

        public bool CanPerformAction(GenericAction action)
        {
            bool result = action.IsActionAvailable();

            if (OnTryAddAction != null) OnTryAddAction(action, ref result);

            if (OnTryAddActionGlobal != null) OnTryAddActionGlobal(this, action, ref result);

            return result;
        }

        public bool CanPerformFreeAction(GenericAction action)
        {
            bool result = action.IsActionAvailable() && action.CanBePerformedAsAFreeAction;

            if (OnTryAddAction != null) OnTryAddAction(action, ref result);

            if (OnTryAddActionGlobal != null) OnTryAddActionGlobal(this, action, ref result);

            return result;
        }

        public void AskPerformFreeAction(GenericAction freeAction, Action callback, bool isForced = false)
        {
            AskPerformFreeAction(new List<GenericAction> { freeAction }, callback, isForced);
        }
        // TODO: move actions list into subphase
        public void AskPerformFreeAction(List<GenericAction> freeActions, Action callback, bool isForced = false)
        {
            GenerateAvailableFreeActionsList(freeActions);

            Triggers.RegisterTrigger(
                new Trigger()
                {
                    Name = "Free action",
                    TriggerOwner = Phases.CurrentPhasePlayer,
                    TriggerType = TriggerTypes.OnFreeAction,
                    EventHandler = delegate {
                        FreeActionDecisonSubPhase newSubPhase = (FreeActionDecisonSubPhase) Phases.StartTemporarySubPhaseNew
                        (
                            "Free action decision",
                            typeof(FreeActionDecisonSubPhase),
                            delegate {
                                var phase = Phases.CurrentSubPhase as FreeActionDecisonSubPhase;
                                if (phase != null && phase.ActionWasPerformed)
                                {
                                    Actions.TakeActionFinish(
                                        delegate { Actions.EndActionDecisionSubhase(
                                            delegate { FinishFreeActionDecision(callback); }
                                        );}
                                    );
                                }
                                else FinishFreeActionDecision(callback);
                            }
                        );
                        newSubPhase.ShowSkipButton = !isForced;
                        newSubPhase.IsForced = isForced;
                        newSubPhase.Start();
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

        public void AddAvailableAction(GenericAction action)
        {
            if (CanPerformAction(action))
            {
                if (!AvailableActionsList.Any(n => n.GetType() == action.GetType() && n.IsRed == action.IsRed))
                {
                    AvailableActionsList.Add(action);
                }
            }
        }

        public void AddAvailableFreeAction(GenericAction action)
        {
            if (CanPerformFreeAction(action))
            {
                if (!AvailableFreeActionsList.Any(n => n.GetType() == action.GetType() && n.IsRed == action.IsRed))
                {
                    AvailableFreeActionsList.Add(action);
                }
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

        public bool IsAlreadyExecutedAction<T>() where T : GenericAction
        {
            bool result = false;
            result = AlreadyExecutedActions.Any(a => a is T);            
            return result;
        }

        // ACTION EFFECTS

        public void GenerateAvailableDiceModifications()
        {
            AvailableDiceModifications = new List<GenericAction>(); ;

            //OLD
            foreach (var token in Tokens.GetAllTokens())
            {
                GenericAction action = token.GetAvailableEffects();
                if (action != null) AddAvailableDiceModification(action);
            }

            if (OnGenerateDiceModifications != null) OnGenerateDiceModifications(this);

            if (OnGenerateDiceModificationsGlobal != null) OnGenerateDiceModificationsGlobal(this);
        }

        public void AddAvailableDiceModification(GenericAction action)
        {
            if (NotAlreadyAddedSameDiceModification(action) && CanUseDiceModification(action))
            {
                AvailableDiceModifications.Add(action);
            }
        }

        private bool NotAlreadyAddedSameDiceModification(GenericAction action)
        {
            // Return true if AvailableActionEffects doesn't contain action of the same type
            return AvailableDiceModifications.FirstOrDefault(n => n.GetType() == action.GetType()) == null;
        }

        public void AddAlreadyExecutedDiceModification(GenericAction action)
        {
            if (!action.CanBeUsedFewTimes) AlreadyExecuteDiceModifications.Add(action);
        }

        public void RemoveAlreadyExecutedDiceModification(GenericAction action)
        {
            AlreadyExecuteDiceModifications.RemoveAll(a => a.GetType() == action.GetType());
        }

        public void ClearAlreadyExecutedDiceModifications()
        {
            AlreadyExecuteDiceModifications = new List<GenericAction>();
        }

        public bool CanUseDiceModification(GenericAction action)
        {
            bool result = true;

            if (!action.IsDiceModificationAvailable()) result = false;

            if (IsDiceModificationAlreadyExecuted(action)) result = false;

            if (result)
            {
                if (OnTryAddAvailableDiceModification != null) OnTryAddAvailableDiceModification(this, action, ref result);

                if (OnTryAddAvailableDiceModificationGlobal != null) OnTryAddAvailableDiceModificationGlobal(this, action, ref result);
            }

            return result;
        }

        private bool IsDiceModificationAlreadyExecuted(GenericAction action)
        {
            bool result = false;

            foreach (var alreadyExecuedAction in AlreadyExecuteDiceModifications)
            {
                if (alreadyExecuedAction.DiceModificationName == action.DiceModificationName)
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        public List<GenericAction> GetAvailableDiceModifications()
        {
            return AvailableDiceModifications;
        }

        // COMPARE DICE RESULTS ACTION EFFECTS

        public void GenerateAvailableCompareResultsEffectsList()
        {
            AvailableDiceModificationsCompareResults = new List<GenericAction>();

            if (OnGenerateDiceModificationsCompareResults != null) OnGenerateDiceModificationsCompareResults(this);

            if (OnGenerateDiceModificationsCompareResultsGlobal != null) OnGenerateDiceModificationsCompareResultsGlobal(this);
        }

        public void AddAvailableCompareResultsEffect(GenericAction action)
        {
            if (CanUseCompareResultsEffect(action))
            {
                AvailableDiceModificationsCompareResults.Add(action);
            }
        }

        public void AddAlreadyExecutedCompareResultsEffect(GenericAction action)
        {
            AlreadyExecutedDiceModificationsCompareResults.Add(action);
        }

        public void RemoveAlreadyExecutedCompareResultsEffect(GenericAction action)
        {
            AlreadyExecutedDiceModificationsCompareResults.RemoveAll(a => a.GetType() == action.GetType());
        }

        public bool CanUseCompareResultsEffect(GenericAction action)
        {
            bool result = true;

            if (!action.IsDiceModificationAvailable()) result = false;

            if (IsCompareResultsEffectAlreadyExecuted(action)) result = false;

            if (result)
            {
                if (OnTryAddDiceModificationCompareResults != null) OnTryAddDiceModificationCompareResults(action, ref result);
            }

            return result;
        }

        private bool IsCompareResultsEffectAlreadyExecuted(GenericAction action)
        {
            bool result = false;

            foreach (var alreadyExecuedCompareResultsEffect in AlreadyExecutedDiceModificationsCompareResults)
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
            return AvailableDiceModificationsCompareResults;
        }

        // OPPOSITE ACTION EFFECTS

        public void GenerateDiceModificationsOpposite()
        {
            AvailableDiceModificationsOpposite = new List<GenericAction>();

            if (OnGenerateDiceModificationsOpposite != null) OnGenerateDiceModificationsOpposite(this);

            if (OnGenerateDiceModificationsOppositeGlobal != null) OnGenerateDiceModificationsOppositeGlobal(this);
        }

        public void AddDiceModificationOpposite(GenericAction action)
        {
            if (CanUseDiceModificationOpposite(action))
            {
                AvailableDiceModificationsOpposite.Add(action);
            }
        }

        public void AddAlreadyExecutedDiceModificationsOpposite(GenericAction action)
        {
            AlreadyExecutedDiceModificationsOpposite.Add(action);
        }

        public void RemoveAlreadyExecutedDiceModificationsOpposite(GenericAction action)
        {
            AlreadyExecutedDiceModificationsOpposite.RemoveAll(a => a.GetType() == action.GetType());
        }

        public void ClearAlreadyExecutedDiceModificationsOpposite()
        {
            AlreadyExecutedDiceModificationsOpposite = new List<GenericAction>();
        }

        public void ClearAlreadyExecutedDiceModificationsCompareResults()
        {
            AlreadyExecutedDiceModificationsCompareResults = new List<GenericAction>();
        }

        public bool CanUseDiceModificationOpposite(GenericAction action)
        {
            bool result = true;

            if (!action.IsDiceModificationAvailable()) result = false;

            if (IsAlreadyExecutedDiceModificationOpposite(action)) result = false;

            if (result)
            {
                if (OnTryAddDiceModificationOpposite != null) OnTryAddDiceModificationOpposite(this, action, ref result);
            }

            return result;
        }

        private bool IsAlreadyExecutedDiceModificationOpposite(GenericAction action)
        {
            bool result = false;

            foreach (var alreadyExecuedOppositeAction in AlreadyExecutedDiceModificationsOpposite)
            {
                if (alreadyExecuedOppositeAction.GetType() == action.GetType())
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        public List<GenericAction> GetDiceModificationsOpposite()
        {
            return AvailableDiceModificationsOpposite;
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

        // Jam action

        public void CallJamTargetIsSelected(GenericShip targetShip, Action callback)
        {
            if (OnJamTargetIsSelected != null) OnJamTargetIsSelected(targetShip);

            Triggers.ResolveTriggers(TriggerTypes.OnJamTargetIsSelected, callback);
        }

        // Reroll is confirmed

        public void CallRerollIsConfirmed(Action callback)
        {
            if (OnRerollIsConfirmed != null) OnRerollIsConfirmed(this);

            Triggers.ResolveTriggers(TriggerTypes.OnRerollIsConfirmed, callback);
        }

        // Decloak

        public void CallDecloak(Action callback)
        {
            if (OnDecloak != null) OnDecloak();

            Triggers.ResolveTriggers(TriggerTypes.OnDecloak, callback);
        }

    }

}

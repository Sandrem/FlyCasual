﻿using System;
using System.Collections.Generic;
using Ship;
using SubPhases;
using UnityEngine;
using Upgrade;
using Players;
using System.Linq;
namespace Abilities
{
    public abstract class GenericAbility
    {
        public string Name { get; private set; }

        public bool IsAppliesConditionCard { get; protected set; }
        protected bool IsAbilityUsed { get; set; }

        private object hostReal;
        public object HostReal
        {
            get { return hostReal; }
            private set { hostReal = value; }
        }

        private GenericShip hostShip;
        public GenericShip HostShip
        {
            get { return hostShip; }
            private set { hostShip = value; }
        }

        private GenericUpgrade hostUpgrade;
        public GenericUpgrade HostUpgrade
        {
            get { return hostUpgrade; }
            private set { hostUpgrade = value; }
        }

        public string HostName
        {
            get
            {
                return HostUpgrade != null ? HostUpgrade.Name : HostShip.PilotName;
            }
        }

        public string HostImageUrl
        {
            get
            {
                return HostUpgrade != null ? HostUpgrade.ImageUrl : HostShip.ImageUrl;
            }
        }

        public virtual void Initialize(GenericShip hostShip)
        {
            HostReal = hostShip;
            HostShip = hostShip;
            Name = HostShip.PilotName + "'s ability";

            ActivateAbility();
        }

        public virtual void Initialize(GenericUpgrade hostUpgrade)
        {
            HostReal = hostUpgrade;
            HostShip = hostUpgrade.Host;
            HostUpgrade = hostUpgrade;
            Name = hostUpgrade.Name + "'s ability";
        }

        // ACTIVATE AND DEACTIVATE

        public abstract void ActivateAbility();

        public abstract void DeactivateAbility();

        // REGISTER TRIGGER

        protected Trigger RegisterAbilityTrigger(TriggerTypes triggerType, EventHandler eventHandler, System.EventArgs e = null)
        {
            var trigger = new Trigger()
            {
                Name = Name,
                TriggerType = triggerType,
                TriggerOwner = HostShip.Owner.PlayerNo,
                EventHandler = eventHandler,
                Sender = hostReal,
                EventArgs = e
            };
            Triggers.RegisterTrigger(trigger);
            return trigger;
        }

        // DECISION USE ABILITY YES/NO

        protected bool alwaysUseAbility;

        protected void AskToUseAbility(Func<bool> useByDefault, EventHandler useAbility, EventHandler dontUseAbility = null, Action callback = null, bool showAlwaysUseOption = false, string infoText = null)
        {
            if (dontUseAbility == null) dontUseAbility = DontUseAbility;

            if (callback == null) callback = Triggers.FinishTrigger;

            DecisionSubPhase pilotAbilityDecision = (DecisionSubPhase) Phases.StartTemporarySubPhaseNew(
                Name,
                typeof(AbilityDecisionSubphase),
                callback
            );

            pilotAbilityDecision.InfoText = infoText ?? "Use " + Name + "?";

            pilotAbilityDecision.RequiredPlayer = HostShip.Owner.PlayerNo;

            pilotAbilityDecision.AddDecision("Yes", useAbility);
            pilotAbilityDecision.AddDecision("No", dontUseAbility);
            if (showAlwaysUseOption) pilotAbilityDecision.AddDecision("Always", delegate { SetAlwaysUse(useAbility); });

            pilotAbilityDecision.DefaultDecisionName = (useByDefault()) ? "Yes" : "No";

            pilotAbilityDecision.ShowSkipButton = true;

            pilotAbilityDecision.Start();
        }

        private class AbilityDecisionSubphase : DecisionSubPhase { }

        private void DontUseAbility(object sender, System.EventArgs e)
        {
            DecisionSubPhase.ConfirmDecision();
        }

        protected bool AlwaysUseByDefault()
        {
            return true;
        }

        protected bool NeverUseByDefault()
        {
            return false;
        }

        protected void SetAlwaysUse(EventHandler useAbility)
        {
            alwaysUseAbility = true;
            useAbility(null, null);
        }

        // SELECT SHIP AS TARGET OF ABILITY

        protected GenericShip TargetShip;

        protected void SelectTargetForAbility(Action selectTargetAction, Func<GenericShip, bool> filterTargets, Func<GenericShip, int> getAiPriority, PlayerNo subphaseOwnerPlayerNo, bool showSkipButton = true, Action customCallback = null, string name = null, string description = null, string imageUrl = null)
        {
            SelectTargetForAbility(selectTargetAction, filterTargets, getAiPriority, subphaseOwnerPlayerNo, name, description, imageUrl, showSkipButton, customCallback);
        }

        protected void SelectTargetForAbility(Action selectTargetAction, Func<GenericShip, bool> filterTargets, Func<GenericShip, int> getAiPriority, PlayerNo subphaseOwnerPlayerNo, string name, string description, string imageUrl, bool showSkipButton = true, Action customCallback = null)
        {
            if (customCallback == null) customCallback = Triggers.FinishTrigger;

            Selection.ChangeActiveShip("ShipId:" + HostShip.ShipId);

            SelectShipSubPhase selectTargetSubPhase = (SelectShipSubPhase)Phases.StartTemporarySubPhaseNew(
                name,
                typeof(AbilitySelectTarget),
                customCallback
            );

            selectTargetSubPhase.PrepareByParameters(
                delegate { SelectShipForAbility(selectTargetAction); },
                filterTargets,
                getAiPriority,
                subphaseOwnerPlayerNo,
                showSkipButton,
                name,
                description,
                imageUrl
            );

            selectTargetSubPhase.Start();
        }

        protected bool FilterByTargetType(GenericShip ship, List<TargetTypes> targetTypes)
        {
            return FilterByTargetType(ship, targetTypes.ToArray());
        }

        protected bool FilterByTargetType(GenericShip ship, params TargetTypes[] targetTypes)
        {
            bool result = false;

            if (targetTypes.Contains(TargetTypes.Enemy) && ship.Owner.PlayerNo != Selection.ThisShip.Owner.PlayerNo) result = true;

            if (targetTypes.Contains(TargetTypes.This) && ship.ShipId == Selection.ThisShip.ShipId) result = true;

            if (targetTypes.Contains(TargetTypes.OtherFriendly) && ship.Owner.PlayerNo == Selection.ThisShip.Owner.PlayerNo && ship.ShipId != Selection.ThisShip.ShipId) result = true;

            return result;
        }

        protected bool FilterTargetsByRange(GenericShip ship, int minRange, int maxRange)
        {
            bool result = true;

            if ((Phases.CurrentSubPhase as SelectShipSubPhase) == null || (Phases.CurrentSubPhase as SelectShipSubPhase).CanMeasureRangeBeforeSelection)
            {
                BoardTools.DistanceInfo distanceInfo = new BoardTools.DistanceInfo(Selection.ThisShip, ship);
                if (distanceInfo.Range < minRange) return false;
                if (distanceInfo.Range > maxRange) return false;
            }

            return result;
        }

        protected bool TargetsForAbilityExist(Func<GenericShip, bool> filter)
        {
            Selection.ChangeActiveShip("ShipId:" + HostShip.ShipId);

            return Roster.AllShips.Values.FirstOrDefault(n => filter(n)) != null;
        }

        private void SelectShipForAbility(Action selectTargetAction)
        {
            MovementTemplates.ReturnRangeRuler();

            TargetShip = (Phases.CurrentSubPhase as SelectShipSubPhase).TargetShip;
            selectTargetAction();
        }

        private class AbilitySelectTarget: SelectShipSubPhase
        {
            public override void RevertSubPhase() { }

            public override void SkipButton()
            {
                UI.HideSkipButton();
                FinishSelection();
            }
        }

        protected void ClearIsAbilityUsedFlag()
        {
            IsAbilityUsed = false;
        }

        protected void SetIsAbilityIsUsed(GenericShip ship)
        {
            IsAbilityUsed = true;
        }

        // DICE CHECKS

        protected DiceRoll DiceCheckRoll;

        public void PerformDiceCheck(string description, DiceKind diceKind, int count, Action finish, Action callback)
        {
            Phases.CurrentSubPhase.Pause();

            Selection.ActiveShip = HostShip;

            AbilityDiceCheck subphase = Phases.StartTemporarySubPhaseNew<AbilityDiceCheck>(
                description,
                callback
            );

            subphase.DiceKind = diceKind;
            subphase.DiceCount = count;
            subphase.AfterRoll = delegate { AfterRollWrapper(finish); };

            subphase.Start();
        }

        private void AfterRollWrapper(Action callback)
        {
            AbilityDiceCheck subphase = Phases.CurrentSubPhase as AbilityDiceCheck;
            subphase.HideDiceResultMenu();
            DiceCheckRoll = subphase.CurrentDiceRoll;

            callback();
        }

        protected class AbilityDiceCheck : DiceRollCheckSubPhase
        {

            public static void ConfirmCheckNoCallback()
            {
                Phases.FinishSubPhase(Phases.CurrentSubPhase.GetType());
                Phases.CurrentSubPhase.Resume();
            }

            public static void ConfirmCheck()
            {
                Action callback = Phases.CurrentSubPhase.CallBack;
                ConfirmCheckNoCallback();
                callback();
            }

        };
    }
}

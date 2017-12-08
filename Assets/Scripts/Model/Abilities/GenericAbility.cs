using System;
using System.Collections.Generic;
using Ship;
using SubPhases;
using UnityEngine;
using Upgrade;

namespace Abilities
{
    public class GenericAbility
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

        public virtual void Initialize(GenericShip hostShip)
        {
            HostReal = hostShip;
            HostShip = hostShip;
            Name = HostShip.PilotName + "'s ability";
        }

        public virtual void Initialize(GenericUpgrade hostUpgrade)
        {
            HostReal = hostUpgrade;
            HostShip = hostUpgrade.Host;
            HostUpgrade = hostUpgrade;
            Name = hostUpgrade.Name + "'s ability";
        }

        // ACTIVATE AND DEACTIVATE

        public virtual void ActivateAbility() { }

        public virtual void DeactivateAbility() { }

        // REGISTER TRIGGER

        protected void RegisterAbilityTrigger(TriggerTypes triggerType, EventHandler eventHandler, System.EventArgs e = null)
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = Name,
                TriggerType = triggerType,
                TriggerOwner = HostShip.Owner.PlayerNo,
                EventHandler = eventHandler,
                Sender = hostReal,
                EventArgs = e
            });
        }

        // DECISION USE ABILITY YES/NO

        protected bool alwaysUseAbility;

        protected void AskToUseAbility(Func<bool> useByDefault, EventHandler useAbility, EventHandler dontUseAbility = null, Action callback = null, bool showAlwaysUseOption = false)
        {
            if (dontUseAbility == null) dontUseAbility = DontUseAbility;

            if (callback == null) callback = Triggers.FinishTrigger;

            DecisionSubPhase pilotAbilityDecision = (DecisionSubPhase) Phases.StartTemporarySubPhaseNew(
                Name,
                typeof(AbilityDecisionSubphase),
                callback
            );

            pilotAbilityDecision.InfoText = "Use " + Name + "?";

            pilotAbilityDecision.RequiredPlayer = HostShip.Owner.PlayerNo;

            pilotAbilityDecision.AddDecision("Yes", useAbility);
            pilotAbilityDecision.AddDecision("No", dontUseAbility);
            if (showAlwaysUseOption) pilotAbilityDecision.AddDecision("Always", delegate { SetAlwaysUse(useAbility); });

            pilotAbilityDecision.DefaultDecision = (useByDefault()) ? "Yes" : "No";

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
            return true;
        }

        protected void SetAlwaysUse(EventHandler useAbility)
        {
            alwaysUseAbility = true;
            useAbility(null, null);
        }

        // SELECT SHIP AS TARGET OF ABILITY

        protected GenericShip TargetShip;

        protected void SelectTargetForAbility(System.Action selectTargetAction, List<TargetTypes> targetTypes, Vector2 rangeLimits, Action callback = null, bool showSkipButton = true)
        {
            if (callback == null) callback = Triggers.FinishTrigger;

            Selection.ThisShip = HostShip;

            SelectShipSubPhase selectTargetSubPhase = (SelectShipSubPhase) Phases.StartTemporarySubPhaseNew(
                "Select target for " + Name,
                typeof(AbilitySelectTarget),
                callback
            );

            selectTargetSubPhase.PrepareByParameters(
                delegate { SelectShipForAbility(selectTargetAction); },
                targetTypes,
                rangeLimits,
                showSkipButton
            );

            selectTargetSubPhase.Start();
        }

        private void SelectShipForAbility(System.Action selectTargetAction)
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
                FinishSelection();
            }
        }
    }
}

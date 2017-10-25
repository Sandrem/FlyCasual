using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{

    public partial class GenericShip
    {
        /**
         * Set this variable is using register pilot abilities.
         */
        protected TriggerTypes triggerTypes;

        /**
         * The name of the pilot ability.
         */
        protected string PilotAbilityName;
        /**
         * A check if the pilot's ability was used.
         */
        public bool abilityUsed = false;

        // ===== Methods Related to Decision Pilot Abilities ====== //
        // ===== Users can extend these abilities for customization ==== //

        /**
         * Implement if the decision is conditional.
         * @param sender the paramater passed from the trigger.
         * @return true as the default value.
         */
        protected virtual bool ShouldShowDecision(object sender){
            return true;
        }

        /**
         * Remove the pilot ability.
         * Implement for any additional requirements.
         */
        protected virtual void RemovePilotDecisionAbility ( GenericShip genericShip) { 
            this.abilityUsed = false;
        }

        public virtual void UsePilotAbility(SubPhases.PilotDecisionSubPhase subPhase)
        {
            this.abilityUsed = true;
        }

        // ===== =========================================== ====== //
        /**
         * Register a decision for using the pilot's ability.
         * Requires the pilotAbilityPhaseStart and pilotAbilityPhase end to be set!!!
         * @param triggerTypes the trigger for when the decision will be started.
         */
        protected void setupDecisionPilotAbility(TriggerTypes triggerTypes)
        {
            this.triggerTypes = triggerTypes;
            this.PilotAbilityName = PilotName + " Ability";
        }

        /**
         * Registers the pilot dicision ability.
         */
        protected void RegisterPilotDecisionAbility (GenericShip genericShip)
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = this.PilotAbilityName,
                TriggerOwner = this.Owner.PlayerNo,
                TriggerType = this.triggerTypes,
                EventHandler = ShowDecision
            });
        }

        protected void ShowDecision(object sender, System.EventArgs e)
        {
            if (ShouldShowDecision (sender)) {
                Selection.ThisShip = this;
                Phases.StartTemporarySubPhase (
                    "Pilot Ability Decision",
                    typeof(SubPhases.PilotDecisionSubPhase),
                    Triggers.FinishTrigger
                );
            } else {
                Triggers.FinishTrigger ();
            }
        }
    }
}

namespace SubPhases
{
    public class PilotDecisionSubPhase : DecisionSubPhase
    {
        public override void Prepare()
        {
            infoText = "Use Pilot Ability?";

            AddDecision("Use Pilot Ability", UseAbility);
            AddDecision("Cancel", DoNotUseAbility );

            defaultDecision = ShouldUsePilotAbility() ? "Use Pilot Ability" : "Cancel";
        }

        private bool ShouldUsePilotAbility()
        {
            return Actions.HasTarget(Selection.ThisShip);
        }

        private void UseAbility(object sender, System.EventArgs e) { 
            Ship.GenericShip ship = (Ship.GenericShip)Selection.ThisShip;
            ship.UsePilotAbility(this);
        }

        private void DoNotUseAbility(object sender, System.EventArgs e)
        {
            ConfirmDecision();
        }

        public void ConfirmDecision()
        {
            Phases.FinishSubPhase(this.GetType());
            CallBack();
        }
    }
}
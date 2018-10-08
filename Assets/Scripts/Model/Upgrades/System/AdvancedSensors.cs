﻿using Upgrade;
using Ship;
using GameModes;
using Abilities;
using Tokens;
using System;
using System.Collections;
using System.Collections.Generic;
using RuleSets;

namespace UpgradesList
{
    public class AdvancedSensors : GenericUpgrade, ISecondEditionUpgrade
    {
        public AdvancedSensors() : base()
        {
            Types.Add(UpgradeType.System);
            Name = "Advanced Sensors";
            Cost = 3;

            UpgradeAbilities.Add (new AdvancedSensorsAbility ());
        }

        public void AdaptUpgradeToSecondEdition()
        {
            Cost = 8;

            SEImageNumber = 23;
        }
    }
}

namespace Abilities
{
    public class AdvancedSensorsAbility : GenericAbility
    {

        // Immediately before you reveal your maneuver, you may perform 1 free action.
        // If you use this ability, you must skip your 'Perform Action' step during
        // this round.
        public override void ActivateAbility()
        {
            HostShip.OnManeuverIsReadyToBeRevealed += RegisterAdvancedSensors;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnManeuverIsReadyToBeRevealed -= RegisterAdvancedSensors;
        }

        private void RegisterAdvancedSensors(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnManeuverIsReadyToBeRevealed, ShowUseAdvancedSensorsDecision);
        }

        private void ShowUseAdvancedSensorsDecision(object sender, System.EventArgs e)
        {
            // give user the option to use ability
            AskToUseAbility (AlwaysUseByDefault, UseAdvancedSensors);
        }

        private void UseAdvancedSensors(object sender, System.EventArgs e)
        {
            List<ActionsList.GenericAction> actions = HostShip.GetAvailableActions();

            HostShip.AskPerformFreeAction(actions, SubPhases.DecisionSubPhase.ConfirmDecision);
            // if ability is used, skipped Perform Action
            HostShip.IsSkipsActionSubPhase = true;
        }

    }
}

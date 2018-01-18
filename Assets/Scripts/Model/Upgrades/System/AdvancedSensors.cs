using Upgrade;
using Ship;
using GameModes;
using Abilities;
using Tokens;
using System;
using System.Collections;
using System.Collections.Generic;

namespace UpgradesList
{
    public class AdvancedSensors : GenericUpgrade
    {
        public AdvancedSensors() : base()
        {
            Type = UpgradeType.System;
            Name = "Advanced Sensors";
            Cost = 3;

            UpgradeAbilities.Add (new AdvancedSensorsAbility ());
        }
    }
}

namespace Abilities
{
    public class AdvancedSensorsAbility : GenericAbility
    {
        private bool FirstTimeActionListToBeGenerated = true;

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
            // in case the actions list was never generated, force it. It may happen if the Advanced Sensors
            // is always selected and the ship never goes through the Action Subphase
            if (FirstTimeActionListToBeGenerated) {
                HostShip.GenerateAvailableActionsList ();
                FirstTimeActionListToBeGenerated = false;
            }
            List<ActionsList.GenericAction> actions = HostShip.GetAvailableActionsList();
            HostShip.AskPerformFreeAction(actions, SubPhases.DecisionSubPhase.ConfirmDecision);
            // if ability is used, skipped Perform Action
            HostShip.IsSkipsActionSubPhase = true;
        }

    }
}

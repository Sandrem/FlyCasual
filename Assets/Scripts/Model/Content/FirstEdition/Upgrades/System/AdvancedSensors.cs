using Ship;
using Upgrade;
using ActionsList;
using System;
using System.Collections.Generic;

namespace UpgradesList.FirstEdition
{
    public class AdvancedSensors : GenericUpgrade
    {
        public AdvancedSensors() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Advanced Sensors",
                UpgradeType.System,
                cost: 3,
                abilityType: typeof(Abilities.FirstEdition.AdvancedSensorsAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
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
            AskToUseAbility(AlwaysUseByDefault, UseAdvancedSensors);
        }

        private void UseAdvancedSensors(object sender, System.EventArgs e)
        {
            List<GenericAction> actions = HostShip.GetAvailableActions();

            HostShip.AskPerformFreeAction(actions, SubPhases.DecisionSubPhase.ConfirmDecision);
            // if ability is used, skipped Perform Action
            HostShip.IsSkipsActionSubPhase = true;
        }

    }
}
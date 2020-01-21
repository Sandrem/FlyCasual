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
                UpgradeType.Sensor,
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
            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                AlwaysUseByDefault,
                UseAdvancedSensors,
                descriptionLong: "Do you want to perform 1 action? (If you do, you cannot perform another action during your activation)",
                imageHolder: HostUpgrade
            );
        }

        private void UseAdvancedSensors(object sender, System.EventArgs e)
        {
            List<GenericAction> actions = HostShip.GetAvailableActions();

            // if ability is used, skipped Perform Action
            HostShip.IsSkipsActionSubPhase = true;

            HostShip.AskPerformFreeAction(
                actions,
                SubPhases.DecisionSubPhase.ConfirmDecision,
                HostUpgrade.UpgradeInfo.Name,
                "You may perform 1 action. You cannot perform another action during your activation.",
                HostUpgrade
            );
        }

    }
}
using Ship;
using Upgrade;
using ActionsList;
using System;
using System.Collections.Generic;

namespace UpgradesList.SecondEdition
{
    public class AdvancedSensors : GenericUpgrade
    {
        public AdvancedSensors() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Advanced Sensors",
                UpgradeType.System,
                cost: 10,
                abilityType: typeof(Abilities.SecondEdition.AdvancedSensorsAbility),
                seImageNumber: 23
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class AdvancedSensorsAbility : GenericAbility
    {

        // After you reveal your dial, you may perform 1 action.
        // If you do, you cannot perform another action during your activation.

        public override void ActivateAbility()
        {
            HostShip.OnManeuverIsRevealed += RegisterAdvancedSensors;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnManeuverIsRevealed -= RegisterAdvancedSensors;
        }

        private void RegisterAdvancedSensors(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnManeuverIsRevealed, UseAdvancedSensors);
        }

        private void UseAdvancedSensors(object sender, System.EventArgs e)
        {
            Messages.ShowInfoToHuman(HostUpgrade.UpgradeInfo.Name + ": You can perform an action");

            HostShip.OnActionIsPerformed += SkipActionsUntilEndOfActivation;
            HostShip.OnActionIsSkipped += SkipAbility;

            List<GenericAction> actions = HostShip.GetAvailableActions();
            HostShip.AskPerformFreeAction(actions, Triggers.FinishTrigger);
        }

        private void SkipAbility(GenericShip ship)
        {
            Messages.ShowInfoToHuman(HostUpgrade.UpgradeInfo.Name + ": Action is skipped");

            HostShip.OnActionIsPerformed -= SkipActionsUntilEndOfActivation;
            HostShip.OnActionIsSkipped -= SkipAbility;
        }

        private void SkipActionsUntilEndOfActivation(GenericAction action)
        {
            Messages.ShowInfoToHuman(HostUpgrade.UpgradeInfo.Name + ": You cannot perform another actions during your activation");

            HostShip.OnActionIsPerformed -= SkipActionsUntilEndOfActivation;
            HostShip.OnActionIsSkipped -= SkipAbility;

            HostShip.OnTryAddAction += DisallowAction;
            HostShip.OnMovementActivationFinish += ClearRestriction;
        }

        private void DisallowAction(GenericAction action, ref bool isAllowed)
        {
            isAllowed = false;
        }

        private void ClearRestriction(GenericShip ship)
        {
            Messages.ShowInfoToHuman(HostUpgrade.UpgradeInfo.Name + ": You can perform actions as usual");

            HostShip.OnMovementActivationFinish -= ClearRestriction;
            HostShip.OnTryAddAction -= DisallowAction;
        }

    }
}
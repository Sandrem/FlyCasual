using Upgrade;
using Ship;
using GameModes;
using Abilities;
using Tokens;
using System;
using System.Collections;
using System.Collections.Generic;
using RuleSets;
using ActionsList;

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

            UpgradeAbilities.RemoveAll(a => a is AdvancedSensorsAbility);
            UpgradeAbilities.Add(new Abilities.SecondEdition.AdvancedSensorsAbilitySE());
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
            List<GenericAction> actions = HostShip.GetAvailableActions();

            HostShip.AskPerformFreeAction(actions, SubPhases.DecisionSubPhase.ConfirmDecision);
            // if ability is used, skipped Perform Action
            HostShip.IsSkipsActionSubPhase = true;
        }

    }
}

namespace Abilities.SecondEdition
{
    public class AdvancedSensorsAbilitySE : GenericAbility
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
            Messages.ShowInfoToHuman(HostUpgrade.Name + ": You can perform an action");

            HostShip.OnActionIsPerformed += SkipActionsUntilEndOfActivation;
            HostShip.OnActionIsSkipped += SkipAbility;

            List<GenericAction> actions = HostShip.GetAvailableActions();
            HostShip.AskPerformFreeAction(actions, Triggers.FinishTrigger);
        }

        private void SkipAbility(GenericShip ship)
        {
            Messages.ShowInfoToHuman(HostUpgrade.Name + ": Action is skipped");

            HostShip.OnActionIsPerformed -= SkipActionsUntilEndOfActivation;
            HostShip.OnActionIsSkipped -= SkipAbility;
        }

        private void SkipActionsUntilEndOfActivation(GenericAction action)
        {
            Messages.ShowInfoToHuman(HostUpgrade.Name + ": You cannot perform another actions during your activation");

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
            Messages.ShowInfoToHuman(HostUpgrade.Name + ": You can perform actions as usual");

            HostShip.OnMovementActivationFinish -= ClearRestriction;
            HostShip.OnTryAddAction -= DisallowAction;
        }

    }
}

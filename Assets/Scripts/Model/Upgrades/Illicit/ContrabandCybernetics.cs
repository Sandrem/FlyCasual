using Upgrade;
using UnityEngine;
using Ship;
using System.Collections.Generic;
using SubPhases;
using Abilities;
using UpgradesList;
using Tokens;

namespace UpgradesList
{
    public class ContrabandCybernetics : GenericUpgrade
    {
        public ContrabandCybernetics() : base()
        {
            Types.Add(UpgradeType.Illicit);
            Name = "Contraband Cybernetics";
            Cost = 1;

            UpgradeAbilities.Add(new ContrabandCyberneticsAbility());
        }
    }
}

namespace Abilities
{
    public class ContrabandCyberneticsAbility : GenericAbility
    {
        private bool CanPerformActionsWhileStressedOriginal;
        private bool CanPerformRedManeuversWhileStressedOriginal;

        public override void ActivateAbility()
        {
            HostShip.OnMovementActivation += RegisterTrigger;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementActivation -= RegisterTrigger;
        }

        private void RegisterTrigger(GenericShip ship)
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = Name,
                TriggerType = TriggerTypes.OnMovementActivation,
                TriggerOwner = HostShip.Owner.PlayerNo,
                EventHandler = AskUseContrabandCybernetics
            });
        }

        private void AskUseContrabandCybernetics(object sender, System.EventArgs e)
        {
            AskToUseAbility(NeverUseByDefault, ActivateContrabandCyberneticsAbility);
        }

        public void ActivateContrabandCyberneticsAbility(object sender, System.EventArgs e)
        {
            HostShip.OnMovementActivation -= RegisterTrigger;
            Phases.OnEndPhaseStart_NoTriggers += DeactivateContrabandCyberneticsAbility;

            HostShip.Tokens.AssignToken(new StressToken(HostShip), RemoveRestrictions);
        }

        private void RemoveRestrictions()
        {
            CanPerformActionsWhileStressedOriginal = HostShip.CanPerformActionsWhileStressed;
            HostShip.CanPerformActionsWhileStressed = true;

            CanPerformRedManeuversWhileStressedOriginal = HostShip.CanPerformRedManeuversWhileStressed;
            HostShip.CanPerformRedManeuversWhileStressed = true;

            HostUpgrade.TryDiscard(DecisionSubPhase.ConfirmDecision);
        }

        public void DeactivateContrabandCyberneticsAbility()
        {
            Phases.OnEndPhaseStart_NoTriggers -= DeactivateContrabandCyberneticsAbility;

            HostShip.CanPerformActionsWhileStressed = CanPerformActionsWhileStressedOriginal;
            HostShip.CanPerformRedManeuversWhileStressed = CanPerformRedManeuversWhileStressedOriginal;
        }
    }
}
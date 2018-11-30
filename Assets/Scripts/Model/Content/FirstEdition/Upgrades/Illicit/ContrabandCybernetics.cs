using Ship;
using SubPhases;
using System;
using Tokens;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class ContrabandCybernetics : GenericUpgrade
    {
        public ContrabandCybernetics() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Contraband Cybernetics",
                UpgradeType.Illicit,
                cost: 1,
                abilityType: typeof(Abilities.FirstEdition.ContrabandCyberneticsAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class ContrabandCyberneticsAbility : GenericAbility
    {
        private bool CanPerformActionsWhileStressedOriginal;
        private bool CanPerformRedManeuversWhileStressedOriginal;

        public override void ActivateAbility()
        {
            HostShip.OnMovementActivationStart += RegisterTrigger;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementActivationStart -= RegisterTrigger;
        }

        private void RegisterTrigger(GenericShip ship)
        {
            if (IsAbilityCanBeUsed())
            {
                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = Name,
                    TriggerType = TriggerTypes.OnMovementActivation,
                    TriggerOwner = HostShip.Owner.PlayerNo,
                    EventHandler = AskUseContrabandCybernetics
                });
            }
        }

        protected virtual bool IsAbilityCanBeUsed()
        {
            return true;
        }

        private void AskUseContrabandCybernetics(object sender, System.EventArgs e)
        {
            if (IsAbilityCanBeUsed())
            {
                AskToUseAbility(NeverUseByDefault, ActivateContrabandCyberneticsAbility);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        public void ActivateContrabandCyberneticsAbility(object sender, System.EventArgs e)
        {
            HostShip.OnMovementActivationStart -= RegisterTrigger;
            Phases.Events.OnEndPhaseStart_NoTriggers += DeactivateContrabandCyberneticsAbility;

            PayActivationCost(RemoveRestrictions);
        }

        protected virtual void PayActivationCost(Action callback)
        {
            HostShip.Tokens.AssignToken(typeof(StressToken), callback);
        }

        private void RemoveRestrictions()
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            Messages.ShowInfo(HostUpgrade.UpgradeInfo.Name + ": You can perform actions and red maneuvers, even while stressed");

            CanPerformActionsWhileStressedOriginal = HostShip.CanPerformActionsWhileStressed;
            HostShip.CanPerformActionsWhileStressed = true;

            CanPerformRedManeuversWhileStressedOriginal = HostShip.CanPerformRedManeuversWhileStressed;
            HostShip.CanPerformRedManeuversWhileStressed = true;

            FinishAbility();
        }

        protected virtual void FinishAbility()
        {
            HostUpgrade.TryDiscard(Triggers.FinishTrigger);
        }

        public void DeactivateContrabandCyberneticsAbility()
        {
            Phases.Events.OnEndPhaseStart_NoTriggers -= DeactivateContrabandCyberneticsAbility;

            HostShip.CanPerformActionsWhileStressed = CanPerformActionsWhileStressedOriginal;
            HostShip.CanPerformRedManeuversWhileStressed = CanPerformRedManeuversWhileStressedOriginal;
        }
    }
}
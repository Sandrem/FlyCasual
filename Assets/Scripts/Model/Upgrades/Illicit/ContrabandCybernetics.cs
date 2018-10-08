using Upgrade;
using UnityEngine;
using Ship;
using System.Collections.Generic;
using SubPhases;
using Abilities;
using UpgradesList;
using Tokens;
using RuleSets;
using System;

namespace UpgradesList
{
    public class ContrabandCybernetics : GenericUpgrade, ISecondEditionUpgrade
    {
        public ContrabandCybernetics() : base()
        {
            Types.Add(UpgradeType.Illicit);
            Name = "Contraband Cybernetics";
            Cost = 1;

            UpgradeAbilities.Add(new ContrabandCyberneticsAbility());
        }

        public void AdaptUpgradeToSecondEdition()
        {
            Cost = 5;

            UsesCharges = true;
            MaxCharges = 1;

            SEImageNumber = 58;

            UpgradeAbilities.RemoveAll(a => a is ContrabandCyberneticsAbility);
            UpgradeAbilities.Add(new Abilities.SecondEdition.ContrabandCyberneticsAbilitySE());
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
            HostShip.OnMovementActivation -= RegisterTrigger;
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

            Messages.ShowInfo(HostUpgrade.Name + ": You can perform actions and red maneuvers, even while stressed");

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

namespace Abilities.SecondEdition
{
    public class ContrabandCyberneticsAbilitySE : ContrabandCyberneticsAbility
    {
        protected override void PayActivationCost(Action callback)
        {
            HostUpgrade.SpendCharge();
            callback();
        }

        protected override bool IsAbilityCanBeUsed()
        {
            return HostUpgrade.Charges > 0;
        }

        protected override void FinishAbility()
        {
            Triggers.FinishTrigger();
        }
    }
}
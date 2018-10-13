using Upgrade;
using Ship;
using Abilities;
using RuleSets;
using Abilities.SecondEdition;
using Movement;

namespace UpgradesList
{
    public class LeiaOrgana : GenericUpgrade, ISecondEditionUpgrade
    {
        public LeiaOrgana() : base()
        {
            Types.Add(UpgradeType.Crew);
            Name = "Leia Organa";
            Cost = 8;

            isUnique = true;

            UpgradeAbilities.Add(new LeiaOrganaAbility());

            UpgradeRuleType = typeof(SecondEdition);

            UsesCharges = true;
            MaxCharges = 3;
            RegensCharges = true;

            SEImageNumber = 88;
        }

        public void AdaptUpgradeToSecondEdition()
        {
            //
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Rebel;
        }

    }
}

namespace Abilities.SecondEdition
{
    public class LeiaOrganaAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnActivationPhaseStart += CheckRegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActivationPhaseStart -= CheckRegisterAbility;
        }

        private void CheckRegisterAbility(GenericShip ship)
        {
            if (HostUpgrade.Charges >= 3)
            {
                RegisterAbilityTrigger(TriggerTypes.OnActivationPhaseStart, AskLeiaAbility);
            }
        }

        private void AskLeiaAbility(object sender, System.EventArgs e)
        {
            if (HostUpgrade.Charges >= 3)
            {
                AskToUseAbility(NeverUseByDefault, UseLeiaAbility);
            }
            else
            {
                Messages.ShowError(HostUpgrade.NameOriginal + ": Not enough charges");
                Triggers.FinishTrigger();
            }
        }

        private void UseLeiaAbility(object sender, System.EventArgs e)
        {
            HostUpgrade.SpendCharges(3);

            Messages.ShowInfo(HostUpgrade.NameOriginal + ": Ability was activated");

            GenericShip.OnManeuverIsReadyToBeRevealedGlobal += CheckReduceComplexity;
            Phases.Events.OnRoundEnd += ClearAbility;

            SubPhases.DecisionSubPhase.ConfirmDecision();
        }

        private void CheckReduceComplexity(GenericShip ship)
        {
            if (ship.Owner.PlayerNo == HostShip.Owner.PlayerNo && ship.AssignedManeuver.ColorComplexity == MovementComplexity.Complex)
            {
                ship.AssignedManeuver.ColorComplexity = GenericMovement.ReduceComplexity(ship.AssignedManeuver.ColorComplexity);
            }
        }

        private void ClearAbility()
        {
            GenericShip.OnManeuverIsReadyToBeRevealedGlobal -= CheckReduceComplexity;
            Phases.Events.OnRoundEnd -= ClearAbility;
        }
    }
}
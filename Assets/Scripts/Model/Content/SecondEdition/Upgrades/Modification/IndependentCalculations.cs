using ActionsList;
using SubPhases;
using System;
using System.Linq;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class IndependentCalculations : GenericUpgrade
    {
        public IndependentCalculations() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Independent Calculations",
                UpgradeType.Modification,
                cost: 0,
                restriction: new AbilityPresenceRestriction(typeof(Abilities.SecondEdition.NetworkedCalculationsAbility)),
                abilityType: typeof(Abilities.SecondEdition.IndependentCalculationsWrapperAbility),
                isStandardazed: true
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/5c/76/5c762c2d-5ae5-43d2-8791-908c211d0515/swz81_upgrade_independent-calculations.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class IndependentCalculationsWrapperAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            NetworkedCalculationsAbility oldAbility = (NetworkedCalculationsAbility)HostShip.ShipAbilities.First(n => n.GetType() == typeof(NetworkedCalculationsAbility));
            oldAbility.DeactivateAbility();
            HostShip.ShipAbilities.Remove(oldAbility);

            GenericAbility ability = new IndependentCalculationsAbility();
            ability.HostUpgrade = HostUpgrade;
            HostShip.ShipAbilities.Add(ability);
            ability.Initialize(HostShip);
        }

        public override void DeactivateAbility()
        {
            HostShip.ShipAbilities.RemoveAll(n => n.GetType() == typeof(IndependentCalculationsAbility));

            GenericAbility ability = new NetworkedCalculationsAbility();
            ability.HostUpgrade = HostUpgrade;
            HostShip.ShipAbilities.Add(ability);
            ability.Initialize(HostShip);
        }
    }
}

namespace Abilities.SecondEdition
{
    public class IndependentCalculationsAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.BeforeActionIsPerformed += RegisterIndependentCalculationsAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.BeforeActionIsPerformed -= RegisterIndependentCalculationsAbility;
        }

        private void RegisterIndependentCalculationsAbility(GenericAction action, ref bool isFree)
        {
            if (action is CalculateAction && action.Color == Actions.ActionColor.White)
            {
                RegisterAbilityTrigger(TriggerTypes.BeforeActionIsPerformed, AskToUseIndependentCalculationsAbility);
            }
        }

        private void AskToUseIndependentCalculationsAbility(object sender, EventArgs e)
        {
            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                NeverUseByDefault,
                UseIndependentCalculationsAbility,
                descriptionLong: "Do you want to treat action as red to gain 1 additional Calculate token?",
                imageHolder: HostUpgrade
            );
        }

        private void UseIndependentCalculationsAbility(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            HostShip.OnCheckActionComplexity += TreatActionAsRed;

            HostShip.Tokens.AssignToken(
                typeof(Tokens.CalculateToken),
                Triggers.FinishTrigger
            );
        }

        private void TreatActionAsRed(GenericAction action, ref Actions.ActionColor color)
        {
            if (action is CalculateAction && action.Color == Actions.ActionColor.White)
            {
                HostShip.OnCheckActionComplexity -= TreatActionAsRed;
                color = Actions.ActionColor.Red;
            }
        }
    }
}
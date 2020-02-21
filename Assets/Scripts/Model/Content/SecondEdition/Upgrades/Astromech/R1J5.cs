using Upgrade;
using System.Collections.Generic;
using ActionsList;
using Ship;
using System;

namespace UpgradesList.SecondEdition
{
    public class R1J5 : GenericUpgrade
    {
        public R1J5() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "R1-J5",
                UpgradeType.Astromech,
                charges: 3,
                cost: 6,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Resistance),
                abilityType: typeof(Abilities.SecondEdition.R1J5AstromechAbility)
            );

            ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/07ef542a7250abbf79d27526f17ac879.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    // While you have 2 or fewer stress tokens, you can perform actions on damage cards even while stressed.
    // After you repair a damage card with the Ship trait, you may spend 1 Icon charge to repair that card again.

    public class R1J5AstromechAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnCheckCanPerformActionsWhileStressed += ConfirmThatIsPossible;
            HostShip.OnCanPerformActionWhileStressed += CheckTwoOrFewerStress;

            HostShip.OnFaceupDamageCardIsRepaired += CheckSecondRepair;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCanPerformActionWhileStressed -= CheckTwoOrFewerStress;
            HostShip.OnCheckCanPerformActionsWhileStressed -= ConfirmThatIsPossible;

            HostShip.OnFaceupDamageCardIsRepaired -= CheckSecondRepair;
        }


        private void ConfirmThatIsPossible(ref bool isAllowed)
        {
            isAllowed = (HostShip.Tokens.CountTokensByType<Tokens.StressToken>() <= 2) && HostShip.Damage.HasFaceupCards;
        }

        private void CheckTwoOrFewerStress(GenericAction action, ref bool isAllowed)
        {
            isAllowed = (HostShip.Tokens.CountTokensByType<Tokens.StressToken>() <= 2) && (action.IsCritCancelAction);
        }

        private void CheckSecondRepair(GenericDamageCard damageCard)
        {
            if (HostUpgrade.State.Charges > 0 && damageCard.Type == CriticalCardType.Ship)
            {
                RegisterAbilityTrigger(TriggerTypes.OnFaceupDamageCardIsRepaired, AskToRepairAgain);
            }
        }

        private void AskToRepairAgain(object sender, EventArgs e)
        {
            AskToUseAbility(HostUpgrade.UpgradeInfo.Name,
                AlwaysUseByDefault,
                RepairAgain,
                descriptionLong: "Do you want to spend 1 charge to repair this damage card again?",
                imageHolder: HostUpgrade,
                requiredPlayer: HostShip.Owner.PlayerNo
            );
        }

        private void RepairAgain(object sender, EventArgs e)
        {
            HostUpgrade.State.SpendCharge();

            HostShip.Damage.DamageCards.Remove(ActionsHolder.SelectedCriticalHitCard);
            HostShip.CallAfterAssignedDamageIsChanged();

            Messages.ShowInfo("R1-J5 repaired facedown damage card");

            SubPhases.DecisionSubPhase.ConfirmDecision();
        }
    }
}
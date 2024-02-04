using Actions;
using ActionsList;
using System;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class PreciseAstromech : GenericUpgrade
    {
        public PreciseAstromech() : base()
        {
            IsHidden = true;

            UpgradeInfo = new UpgradeCardInfo
            (
                "Precise Astromech",
                UpgradeType.Astromech,
                cost: 0,
                charges: 2,
                abilityType: typeof(Abilities.SecondEdition.PreciseAstromechAbility)
            );

            ImageUrl = "https://i.imgur.com/MtJ6aYt.jpg";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class PreciseAstromechAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnActionIsPerformed += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= CheckAbility;
        }

        private void CheckAbility(GenericAction action)
        {
            if (HostUpgrade.State.Charges > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, AskPerfromRedLockAction);
            }
        }

        private void AskPerfromRedLockAction(object sender, EventArgs e)
        {
            HostShip.BeforeActionIsPerformed += PayChargeCost;

            HostShip.AskPerformFreeAction
            (
                new TargetLockAction() { Color = ActionColor.Red },
                CleanUp,
                descriptionShort: HostUpgrade.UpgradeInfo.Name,
                descriptionLong: "You may spend 1 Charge to perform a red Lock action"
            );
        }

        private void PayChargeCost(GenericAction action, ref bool isFreeAction)
        {
            HostUpgrade.State.SpendCharge();
            HostShip.BeforeActionIsPerformed -= PayChargeCost;
        }

        private void CleanUp()
        {
            HostShip.BeforeActionIsPerformed -= PayChargeCost;
            Triggers.FinishTrigger();
        }
    }
}
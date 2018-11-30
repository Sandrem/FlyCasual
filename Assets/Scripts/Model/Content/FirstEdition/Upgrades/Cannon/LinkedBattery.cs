using Ship;
using System.Linq;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class LinkedBattery : GenericUpgrade
    {
        public LinkedBattery() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Linked Battery",
                UpgradeType.Cannon,
                cost: 2,
                feIsLimited: true,
                restriction: new BaseSizeRestriction(BaseSize.Small),
                abilityType: typeof(Abilities.FirstEdition.LinkedBatteryAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class LinkedBatteryAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += CheckLinkedBatteryAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= CheckLinkedBatteryAbility;
        }

        public void CheckLinkedBatteryAbility(GenericShip ship)
        {
            ship.AddAvailableDiceModification(new ActionsList.LinkedBatteryAction()
            {
                ImageUrl = HostUpgrade.ImageUrl,
                Host = HostShip
            });
        }
    }
}

namespace ActionsList
{
    public class LinkedBatteryAction : GenericAction
    {
        public LinkedBatteryAction()
        {
            Name = DiceModificationName = "Linked Battery";
            IsReroll = true;
        }

        public override void ActionEffect(System.Action callBack)
        {
            DiceRerollManager diceRerollManager = new DiceRerollManager
            {
                NumberOfDiceCanBeRerolled = 1,
                CallBack = callBack
            };
            diceRerollManager.Start();
        }

        public override bool IsDiceModificationAvailable()
        {
            bool result = false;
            if (Combat.AttackStep == CombatStep.Attack)
            {
                if (IsPrimaryWeapon() || IsCannon())
                {
                    result = true;
                }
            }
            return result;
        }

        private bool IsPrimaryWeapon()
        {
            return Combat.ChosenWeapon is PrimaryWeaponClass;
        }

        private bool IsCannon()
        {
            return Combat.ChosenWeapon is GenericSpecialWeapon && (Combat.ChosenWeapon as GenericSpecialWeapon).HasType(UpgradeType.Cannon);
        }

        public override int GetDiceModificationPriority()
        {
            int result = 0;

            if (Combat.AttackStep == CombatStep.Attack && (IsPrimaryWeapon() || IsCannon()))
            {
                if (Combat.DiceRollAttack.Blanks > 0)
                {
                    result = 90;
                }
                else if (Combat.DiceRollAttack.Focuses > 0 && Combat.Attacker.GetDiceModificationsGenerated().Count(n => n.IsTurnsAllFocusIntoSuccess) == 0)
                {
                    result = 90;
                }
                else if (Combat.DiceRollAttack.Focuses > 0)
                {
                    result = 30;
                }
            }

            return result;
        }

    }
}
using Ship;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class Xg1AssaultConfiguration : GenericUpgrade
    {
        public Xg1AssaultConfiguration() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Xg-1 Assault Configuration",
                UpgradeType.Configuration,
                cost: 0,
                restriction: new ShipRestriction(typeof(Ship.SecondEdition.AlphaClassStarWing.AlphaClassStarWing)),
                addSlot: new UpgradeSlot(UpgradeType.Cannon),
                abilityType: typeof(Abilities.SecondEdition.Xg1AssaultConfigurationAbility),
                seImageNumber: 126
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class Xg1AssaultConfigurationAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnWeaponsDisabledCheck += AllowLowCostCannons;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnWeaponsDisabledCheck -= AllowLowCostCannons;
        }

        private void AllowLowCostCannons(ref bool result)
        {
            if (HostShip.Tokens.CountTokensByType(typeof(WeaponsDisabledToken)) != 1) return;

            if (!IsCannonAttack()) return;

            Messages.ShowInfo("The attack using " + HostUpgrade.UpgradeInfo.Name + " is allowed.");
            result = false;
            PrepareAttackDiceCap();
        }

        private bool IsCannonAttack()
        {
            bool result = false;

            GenericSpecialWeapon secondaryWeapon = Combat.ChosenWeapon as GenericSpecialWeapon;
            if (secondaryWeapon != null)
            {
                if (secondaryWeapon.HasType(UpgradeType.Cannon))
                {
                    result = true;
                }
            }

            return result;
        }

        private void PrepareAttackDiceCap()
        {
            HostShip.AfterGotNumberOfAttackDiceCap += SetAttackDiceCap;

            HostShip.OnAttackFinish += RemoveAttackDiceCap;
        }

        private void SetAttackDiceCap(ref int count)
        {
            Messages.ShowInfo(HostShip.PilotInfo.PilotName + " has a disarmed token.  Only 3 dice may be rolled when attacking with " + HostUpgrade.UpgradeInfo.Name + ".");
            if (count > 3) count = 3;
        }

        private void RemoveAttackDiceCap(GenericShip ship)
        {
            HostShip.AfterGotNumberOfAttackDiceCap -= SetAttackDiceCap;

            HostShip.OnAttackFinish -= RemoveAttackDiceCap;
        }
    }
}
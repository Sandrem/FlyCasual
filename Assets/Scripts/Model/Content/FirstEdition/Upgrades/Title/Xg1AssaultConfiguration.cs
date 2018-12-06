using Ship;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class Xg1AssaultConfiguration : GenericUpgrade
    {
        public Xg1AssaultConfiguration() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Xg-1 Assault Configuration",
                UpgradeType.Title,
                cost: 1,
                restriction: new ShipRestriction(typeof(Ship.FirstEdition.AlphaClassStarWing.AlphaClassStarWing)),
                addSlots: new List<UpgradeSlot>()
                {
                    new UpgradeSlot(UpgradeType.Cannon),
                    new UpgradeSlot(UpgradeType.Cannon)
                },
                abilityType: typeof(Abilities.FirstEdition.Xg1AssaultConfigurationAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
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
            GenericSpecialWeapon secondaryWeapon = Combat.ChosenWeapon as GenericSpecialWeapon;
            if (secondaryWeapon != null)
            {
                if (secondaryWeapon.HasType(UpgradeType.Cannon) && secondaryWeapon.UpgradeInfo.Cost <= 2)
                {
                    result = false;
                }
            }
        }
    }
}
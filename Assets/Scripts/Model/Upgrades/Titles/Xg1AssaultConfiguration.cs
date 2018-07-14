using Ship;
using Ship.AlphaClassStarWing;
using Upgrade;
using System.Collections.Generic;
using System;
using Abilities;

namespace UpgradesList
{
    public class Xg1AssaultConfiguration : GenericUpgradeSlotUpgrade
    {
        public Xg1AssaultConfiguration() : base()
        {
            Types.Add(UpgradeType.Title);
            Name = "Xg-1 Assault Configuration";
            Cost = 1;
            AddedSlots = new List<UpgradeSlot>
            {
                new UpgradeSlot(UpgradeType.Cannon),
                new UpgradeSlot(UpgradeType.Cannon)
            };
            UpgradeAbilities.Add(new Xg1AssaultConfigurationAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is AlphaClassStarWing;
        }
    }
}

namespace Abilities
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
            GenericSecondaryWeapon secondaryWeapon = Combat.ChosenWeapon as GenericSecondaryWeapon;
            if (secondaryWeapon != null)
            {
                if (secondaryWeapon.HasType(UpgradeType.Cannon) && secondaryWeapon.Cost <= 2)
                {
                    result = false;
                }
            }
        }
    }
}

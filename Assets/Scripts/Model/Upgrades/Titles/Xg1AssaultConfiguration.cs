using Ship;
using Ship.AlphaClassStarWing;
using Upgrade;
using System.Collections.Generic;

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
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is AlphaClassStarWing;
        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);

            host.OnWeaponsDisabledCheck += AllowLowCostCannons;
        }

        private void AllowLowCostCannons(ref bool result)
        {
            GenericSecondaryWeapon secondaryWeapon = Combat.ChosenWeapon as GenericSecondaryWeapon;
            if (secondaryWeapon != null)
            {
                if (secondaryWeapon.hasType(UpgradeType.Cannon) && secondaryWeapon.Cost <= 2) result = false;
            }
        }
    }
}

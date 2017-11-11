using Ship;
using Ship.AlphaClassStarWing;
using Upgrade;
using System.Collections.Generic;

namespace UpgradesList
{
    public class Os1ArsenalLoadout : GenericUpgradeSlotUpgrade
    {
        public Os1ArsenalLoadout() : base()
        {
            Type = UpgradeType.Title;
            Name = "Os-1 Arsenal Loadout";
            Cost = 2;
            AddedSlots = new List<UpgradeSlot>
            {
                new UpgradeSlot(UpgradeType.Torpedo),
                new UpgradeSlot(UpgradeType.Missile)
            };
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is AlphaClassStarWing;
        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);

            host.OnWeaponsDisabledCheck += AllowLaunchesByTargetLock;
        }

        private void AllowLaunchesByTargetLock(ref bool result)
        {
            GenericSecondaryWeapon secondaryWeapon = Combat.ChosenWeapon as GenericSecondaryWeapon;
            if (secondaryWeapon != null)
            {
                if (secondaryWeapon.Type == UpgradeType.Torpedo || secondaryWeapon.Type == UpgradeType.Missile)
                {
                    if (Actions.HasTargetLockOn(Selection.ThisShip, Selection.AnotherShip))
                    {
                        result = true;
                    }
                };
            }
        }
    }
}

using Ship;
using Ship.AlphaClassStarWing;
using Upgrade;
using System.Collections.Generic;
using System;
using Abilities;

namespace UpgradesList
{
    public class Os1ArsenalLoadout : GenericUpgradeSlotUpgrade
    {
        public Os1ArsenalLoadout() : base()
        {
            Types.Add(UpgradeType.Title);
            Name = "Os-1 Arsenal Loadout";
            Cost = 2;
            AddedSlots = new List<UpgradeSlot>
            {
                new UpgradeSlot(UpgradeType.Torpedo),
                new UpgradeSlot(UpgradeType.Missile)
            };
            UpgradeAbilities.Add(new Os1ArsenalLoadoutAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is AlphaClassStarWing;
        }
    }
}

namespace Abilities
{
    public class Os1ArsenalLoadoutAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnWeaponsDisabledCheck += AllowLaunchesByTargetLock;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnWeaponsDisabledCheck -= AllowLaunchesByTargetLock;
        }

        private void AllowLaunchesByTargetLock(ref bool result)
        {
            GenericSecondaryWeapon secondaryWeapon = Combat.ChosenWeapon as GenericSecondaryWeapon;
            if (secondaryWeapon != null)
            {
                if ((secondaryWeapon.hasType(UpgradeType.Torpedo) || secondaryWeapon.hasType(UpgradeType.Missile)) && Actions.HasTargetLockOn(Selection.ThisShip, Selection.AnotherShip))
                {
                    result = false;
                }
            }
        }
    }
}

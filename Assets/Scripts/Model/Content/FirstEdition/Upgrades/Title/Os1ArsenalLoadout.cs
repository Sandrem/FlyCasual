using Ship;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class Os1ArsenalLoadout : GenericUpgrade
    {
        public Os1ArsenalLoadout() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Os-1 Arsenal Loadout",
                UpgradeType.Title,
                cost: 2,
                restriction: new ShipRestriction(typeof(Ship.FirstEdition.AlphaClassStarWing.AlphaClassStarWing)),
                addSlots: new List<UpgradeSlot>()
                {
                    new UpgradeSlot(UpgradeType.Torpedo),
                    new UpgradeSlot(UpgradeType.Missile)
                },
                abilityType: typeof(Abilities.FirstEdition.Os1ArsenalLoadoutAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
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
            GenericSpecialWeapon secondaryWeapon = Combat.ChosenWeapon as GenericSpecialWeapon;
            if (secondaryWeapon != null)
            {
                if ((secondaryWeapon.HasType(UpgradeType.Torpedo) || secondaryWeapon.HasType(UpgradeType.Missile)) && ActionsHolder.HasTargetLockOn(Selection.ThisShip, Selection.AnotherShip))
                {
                    result = false;
                }
            }
        }
    }
}
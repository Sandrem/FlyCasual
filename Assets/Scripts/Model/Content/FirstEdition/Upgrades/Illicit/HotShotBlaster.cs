using BoardTools;
using Ship;
using System;
using System.Linq;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class HotShotBlaster : GenericSpecialWeapon
    {
        public HotShotBlaster() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "\"Hot Shot\" Blaster",
                UpgradeType.Illicit,
                cost: 3,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 3,
                    minRange: 1,
                    maxRange: 3,
                    canShootOutsideArc: true,
                    discard: true
                )
            );
        }        
    }
}
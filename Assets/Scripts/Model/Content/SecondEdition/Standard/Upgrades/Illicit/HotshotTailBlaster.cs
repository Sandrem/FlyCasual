using BoardTools;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class HotshotTailBlaster : GenericSpecialWeapon
    {
        public HotshotTailBlaster() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Hotshot Tail Blaster",
                UpgradeType.Illicit,
                cost: 2,
                restriction: new BaseSizeRestriction(BaseSize.Medium, BaseSize.Large),
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 2,
                    minRange: 0,
                    maxRange: 1,
                    arc: Arcs.ArcType.Rear,
                    charges: 2
                )
            );

            ImageUrl = "https://infinitearenas.com/xw2/images/upgrades/hotshottailblaster.png";
        }        
    }
}
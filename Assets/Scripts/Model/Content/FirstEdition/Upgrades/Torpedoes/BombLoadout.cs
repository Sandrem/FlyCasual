using Ship;
using System.Collections.Generic;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class BombLoadout : GenericUpgrade
    {
        public BombLoadout() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Bomb Loadout",
                UpgradeType.Torpedo,
                cost: 0,
                feIsLimited: true,
                addSlot: new UpgradeSlot(UpgradeType.Bomb),
                restriction: new ShipRestriction(typeof(Ship.FirstEdition.YWing.YWing))
            );
        }        
    }
}
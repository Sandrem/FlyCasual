using Ship;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class SlaveI : GenericUpgrade
    {
        public SlaveI() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Slave I",
                UpgradeType.Title,
                cost: 0,
                isLimited: true,
                addSlot: new UpgradeSlot(UpgradeType.Torpedo),
                restriction: new ShipRestriction(typeof(Ship.FirstEdition.Firespray31.Firespray31))
            );
        }        
    }
}
using Ship;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class TIEx1 : GenericUpgrade
    {
        public TIEx1() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "TIE/x1",
                UpgradeType.Title,
                cost: 0,
                addSlot: new UpgradeSlot(UpgradeType.System) { CostDecrease = 4 },
                restriction: new ShipRestriction(typeof(Ship.FirstEdition.TIEAdvanced.TIEAdvanced))
            );
        }        
    }
}
using ActionsList;
using Ship;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class RenegadeRefit : GenericUpgrade
    {
        public RenegadeRefit() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Renegade Refit",
                UpgradeType.Torpedo,
                cost: -2,
                addSlot: new UpgradeSlot(UpgradeType.Modification) { MustBeDifferent = true },
                costReductionByType: new Dictionary<UpgradeType, int>() { { UpgradeType.Talent, 1 } },
                restriction: new ShipRestriction(typeof(Ship.FirstEdition.XWing.XWing), typeof(Ship.FirstEdition.UWing.UWing))
            );
        }        
    }
}
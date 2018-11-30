using ActionsList;
using Ship;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class RenegadeRefit : GenericUpgradeSlotUpgrade
    {
        public RenegadeRefit() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Renegade Refit",
                UpgradeType.Torpedo,
                cost: -2,
                addSlot: new UpgradeSlot(UpgradeType.Modification) { MustBeDifferent = true },
                restriction: new ShipRestriction(typeof(Ship.FirstEdition.XWing.XWing), typeof(Ship.FirstEdition.UWing.UWing))
            );

            CostReductionByType = new Dictionary<UpgradeType, int>()
            {
                { UpgradeType.Elite, 1 }
            };
        }        
    }
}
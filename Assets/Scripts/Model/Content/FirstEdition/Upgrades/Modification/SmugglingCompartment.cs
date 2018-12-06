using Ship;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class SmugglingCompartment : GenericUpgrade
    {
        public SmugglingCompartment() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Smuggling Compartment",
                UpgradeType.Modification,
                cost: 0,
                feIsLimitedPerShip: true,
                addSlots: new List<UpgradeSlot>
                {
                    new UpgradeSlot(UpgradeType.Crew),
                    new UpgradeSlot(UpgradeType.Modification) { MaxCost = 3 }
                },
                restriction: new ShipRestriction(typeof(Ship.FirstEdition.YT1300.YT1300), typeof(Ship.FirstEdition.YT2400.YT2400))
            );
        }
    }
}

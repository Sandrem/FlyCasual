using Ship;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class BWingE2 : GenericUpgrade
    {
        public BWingE2() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "B-Wing/E2",
                UpgradeType.Modification,
                cost: 1,
                addSlot: new UpgradeSlot(UpgradeType.Crew),
                restriction: new ShipRestriction(typeof(Ship.FirstEdition.BWing.BWing))
            );
        }
    }
}

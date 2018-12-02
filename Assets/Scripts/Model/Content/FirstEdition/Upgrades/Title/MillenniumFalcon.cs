using Ship;
using Upgrade;
using System.Collections.Generic;
using Actions;
using ActionsList;

namespace UpgradesList.FirstEdition
{
    public class MillenniumFalcon : GenericUpgrade
    {
        public MillenniumFalcon() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Millennium Falcon",
                UpgradeType.Title,
                cost: 1,
                isLimited: true,
                restriction: new ShipRestriction(typeof(Ship.FirstEdition.YT1300.YT1300)),
                addAction: new ActionInfo(typeof(EvadeAction))
            );
        }        
    }
}

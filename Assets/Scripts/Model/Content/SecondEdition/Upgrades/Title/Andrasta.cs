using Actions;
using ActionsList;
using Ship;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class Andrasta : GenericUpgrade
    {
        public Andrasta() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Andrasta",
                UpgradeType.Title,
                cost: 4,
                isLimited: true,
                addSlot: new UpgradeSlot(UpgradeType.Bomb),
                addAction: new ActionInfo(typeof(ReloadAction)),
                restriction: new ShipRestriction(typeof(Ship.FirstEdition.Firespray31.Firespray31)),
                seImageNumber: 146
            );
        }        
    }
}
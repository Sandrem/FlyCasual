using Ship;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class Andrasta : GenericUpgrade
    {
        public Andrasta() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Andrasta",
                UpgradeType.Title,
                cost: 0,
                isLimited: true,
                addSlots: new List<UpgradeSlot>()
                {
                    new UpgradeSlot(UpgradeType.Bomb),
                    new UpgradeSlot(UpgradeType.Bomb)
                },                
                restriction: new ShipRestriction(typeof(Ship.FirstEdition.Firespray31.Firespray31))
            );
        }        
    }
}
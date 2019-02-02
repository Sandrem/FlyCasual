using Ship;
using Upgrade;
using System.Collections.Generic;

namespace UpgradesList.FirstEdition
{
    public class Havoc : GenericUpgrade
    {
        public Havoc() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Havoc",
                UpgradeType.Title,
                cost: 0,          
                addSlots: new List<UpgradeSlot>
                {
                    new UpgradeSlot(UpgradeType.System) {  },
                    new UpgradeSlot(UpgradeType.SalvagedAstromech) { MustBeUnique = true }
                },
                forbidSlot: UpgradeType.Crew,
                restriction: new ShipRestriction(typeof(Ship.FirstEdition.ScurrgH6Bomber.ScurrgH6Bomber))
            );
        }        
    }
}
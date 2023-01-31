using Ship;
using Upgrade;
using System.Collections.Generic;

namespace UpgradesList.SecondEdition
{
    public class Havoc : GenericUpgrade
    {
        public Havoc() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Havoc",
                UpgradeType.Title,
                cost: 0,
                isLimited: true,
                addSlots: new List<UpgradeSlot>
                {
                    new UpgradeSlot(UpgradeType.Sensor),
                    new UpgradeSlot(UpgradeType.Astromech)
                },
                forbidSlot: UpgradeType.Crew,
                restriction: new ShipRestriction(typeof(Ship.SecondEdition.ScurrgH6Bomber.ScurrgH6Bomber)),
                seImageNumber: 147
            );
        }        
    }
}
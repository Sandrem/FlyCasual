using Ship;
using Upgrade;
using System.Collections.Generic;

namespace UpgradesList.FirstEdition
{
    public class TIEShuttle : GenericUpgrade
    {
        public TIEShuttle() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "TIE Shuttle",
                UpgradeType.Title,
                cost: 0,
                addSlots: new List<UpgradeSlot>()
                {
                    new UpgradeSlot(UpgradeType.Crew) { MaxCost = 4 },
                    new UpgradeSlot(UpgradeType.Crew) { MaxCost = 4 }
                },
                forbidSlots: new List<UpgradeType>()
                {
                    UpgradeType.Torpedo,
                    UpgradeType.Missile,
                    UpgradeType.Bomb
                },
                restriction: new ShipRestriction(typeof(Ship.FirstEdition.TIEBomber.TIEBomber))
            );
        }        
    }
}
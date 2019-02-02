using Ship;
using Upgrade;
using System.Collections.Generic;
using ActionsList;
using Actions;

namespace UpgradesList.SecondEdition
{
    public class MistHunter : GenericUpgrade
    {
        public MistHunter() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Mist Hunter",
                UpgradeType.Title,
                cost: 2,       
                isLimited: true,
                addSlot: new UpgradeSlot(UpgradeType.Cannon),
                addAction: new ActionInfo(typeof(BarrelRollAction)),
                restriction: new ShipRestriction(typeof(Ship.SecondEdition.G1AStarfighter.G1AStarfighter)),
                seImageNumber: 151
            );
        }
    }
}
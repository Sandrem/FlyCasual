using Ship;
using System.Collections.Generic;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class HullUpgrade : GenericUpgrade, IVariableCost
    {
        public HullUpgrade() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Hull Upgrade",
                UpgradeType.Modification,
                cost: 3,
                addHull: 1,
                seImageNumber: 73
            );
        }

        public void UpdateCost(GenericShip ship)
        {
            Dictionary<int, int> agilityToCost = new Dictionary<int, int>()
            {
                {0, 2},
                {1, 3},
                {2, 5},
                {3, 7}
            };

            UpgradeInfo.Cost = agilityToCost[ship.ShipInfo.Agility];
        }
    }
}
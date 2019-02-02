using Upgrade;
using Ship;
using System.Collections.Generic;

namespace UpgradesList.SecondEdition
{
    public class ShieldUpgrade : GenericUpgrade, IVariableCost
    {
        public ShieldUpgrade() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Shield Upgrade",
                UpgradeType.Modification,
                cost: 4,
                addShields: 1,
                seImageNumber: 75
            );
        }

        public void UpdateCost(GenericShip ship)
        {
            Dictionary<int, int> agilityToCost = new Dictionary<int, int>()
            {
                {0, 3},
                {1, 4},
                {2, 6},
                {3, 8}
            };

            UpgradeInfo.Cost = agilityToCost[ship.ShipInfo.Agility];
        }
    }
}
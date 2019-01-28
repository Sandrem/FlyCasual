using Actions;
using ActionsList;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class EngineUpgrade : GenericUpgrade, IVariableCost
    {
        public EngineUpgrade() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Engine Upgrade",
                UpgradeType.Modification,
                cost: 4,
                restriction: new ActionBarRestriction(new ActionInfo(typeof(BoostAction), ActionColor.Red)),
                addAction: new ActionInfo(typeof(BoostAction)),
                seImageNumber: 72
            );
        }

        public void UpdateCost(GenericShip ship)
        {
            Dictionary<BaseSize, int> sizeToCost = new Dictionary<BaseSize, int>()
            {
                {BaseSize.Small, 2},
                {BaseSize.Medium, 4},
                {BaseSize.Large, 7},
            };

            UpgradeInfo.Cost = sizeToCost[ship.ShipInfo.BaseSize];
        }
    }
}
using Upgrade;
using System.Collections.Generic;
using ActionsList;
using Ship;
using Actions;

namespace UpgradesList.SecondEdition
{
    public class ExpertHandling : GenericUpgrade, IVariableCost
    {
        public ExpertHandling() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Expert Handling",
                UpgradeType.Talent,
                cost: 2,
                restriction: new ActionBarRestriction(new ActionInfo(typeof(BarrelRollAction), ActionColor.Red)),
                addAction: new ActionInfo(typeof(BarrelRollAction)),
                seImageNumber: 5
            );
        }

        public void UpdateCost(GenericShip ship)
        {
            Dictionary<BaseSize, int> sizeToCost = new Dictionary<BaseSize, int>()
            {
                {BaseSize.Small, 2},
                {BaseSize.Medium, 4},
                {BaseSize.Large, 6},
            };

            UpgradeInfo.Cost = sizeToCost[ship.ShipInfo.BaseSize];
        }
    }
}
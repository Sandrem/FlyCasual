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
                abilityType: typeof(Abilities.FirstEdition.EngineUpgradeAbility),
                restriction: new ActionBarRestriction(new ActionInfo(typeof(BoostAction), ActionColor.Red)),
                seImageNumber: 72
            );
        }

        public void UpdateCost(GenericShip ship)
        {
            Dictionary<BaseSize, int> sizeToCost = new Dictionary<BaseSize, int>()
            {
                {BaseSize.Small, 3},
                {BaseSize.Medium, 6},
                {BaseSize.Large, 9},
            };

            UpgradeInfo.Cost = sizeToCost[ship.ShipInfo.BaseSize];
        }
    }
}
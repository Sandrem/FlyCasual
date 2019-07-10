using Actions;
using ActionsList;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class AngledDeflectors : GenericUpgrade, IVariableCost
    {
        public AngledDeflectors() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Angled Deflectors",
                UpgradeType.Modification,
                cost: 6,
                restrictions: new UpgradeCardRestrictions(
                    new BaseSizeRestriction(BaseSize.Small, BaseSize.Medium), 
                    new StatValueRestriction(
                        StatValueRestriction.Stats.Shields,
                        StatValueRestriction.Conditions.HigherThanOrEqual,
                        1
                    )
                ),
                addAction: new ActionInfo(typeof(ReinforceAction)),
                addShields: -1
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/50/91/5091f169-b8ea-449a-909d-9d8dd39b2efb/swz45_angled-deflectors.png";
        }

        public void UpdateCost(GenericShip ship)
        {
            Dictionary<int, int> agilityToCost = new Dictionary<int, int>()
            {
                {0, 9},
                {1, 6},
                {2, 3},
                {3, 3}
            };

            UpgradeInfo.Cost = agilityToCost[ship.ShipInfo.Agility];
        }
    }
}
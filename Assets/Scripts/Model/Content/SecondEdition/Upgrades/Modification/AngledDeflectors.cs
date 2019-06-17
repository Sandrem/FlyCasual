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
                cost: 4,
                restrictions: new UpgradeCardRestrictions(
                    new BaseSizeRestriction(BaseSize.Small, BaseSize.Medium), 
                    new StatValueRestriction(
                        StatValueRestriction.Stats.Shields,
                        StatValueRestriction.Conditions.HigherThanOrEqual,
                        1
                    )
                ),
                addAction: new ActionInfo(typeof(ReinforceAction)),
                addShields: -1,
                seImageNumber: 75
            );
            FromMod = typeof(Mods.ModsList.UnreleasedContentMod);

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/50/91/5091f169-b8ea-449a-909d-9d8dd39b2efb/swz45_angled-deflectors.png";
        }

        public void UpdateCost(GenericShip ship)
        {
            Dictionary<int, int> agilityToCost = new Dictionary<int, int>()
            {
                {0, 5},
                {1, 4},
                {2, 3},
                {3, 2}
            };

            UpgradeInfo.Cost = agilityToCost[ship.ShipInfo.Agility];
        }
    }
}
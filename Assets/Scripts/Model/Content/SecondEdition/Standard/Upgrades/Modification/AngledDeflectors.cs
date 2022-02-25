using Actions;
using ActionsList;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class AngledDeflectors : GenericUpgrade
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
                addShields: -1
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/50/91/5091f169-b8ea-449a-909d-9d8dd39b2efb/swz45_angled-deflectors.png";
        }
    }
}
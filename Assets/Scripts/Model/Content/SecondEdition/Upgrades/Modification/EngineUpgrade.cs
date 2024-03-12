using Actions;
using ActionsList;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class EngineUpgrade : GenericUpgrade
    {
        public EngineUpgrade() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Engine Upgrade",
                UpgradeType.Modification,
                cost: 3,
                restriction: new ActionBarRestriction(typeof(BoostAction), ActionColor.Red),
                addAction: new ActionInfo(typeof(BoostAction)),
                seImageNumber: 72
            );
        }
    }
}
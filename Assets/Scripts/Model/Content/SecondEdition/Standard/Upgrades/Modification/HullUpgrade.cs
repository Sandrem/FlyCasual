using Ship;
using System.Collections.Generic;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class HullUpgrade : GenericUpgrade
    {
        public HullUpgrade() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Hull Upgrade",
                UpgradeType.Modification,
                cost: 4,
                addHull: 1,
                seImageNumber: 73
            );
        }
    }
}
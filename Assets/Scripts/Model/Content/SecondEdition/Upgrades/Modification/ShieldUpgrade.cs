using Upgrade;
using Ship;
using System.Collections.Generic;

namespace UpgradesList.SecondEdition
{
    public class ShieldUpgrade : GenericUpgrade
    {
        public ShieldUpgrade() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Shield Upgrade",
                UpgradeType.Modification,
                cost: 8,
                addShields: 1,
                seImageNumber: 75
            );
        }
    }
}
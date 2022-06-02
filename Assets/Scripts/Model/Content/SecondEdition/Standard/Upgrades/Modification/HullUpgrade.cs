using Ship;
using System.Collections.Generic;
using Upgrade;
using Content;

namespace UpgradesList.SecondEdition
{
    public class HullUpgrade : GenericUpgrade
    {
        public HullUpgrade() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Hull Upgrade",
                UpgradeType.Modification,
                cost: 6,
                addHull: 1,
                seImageNumber: 73,
                legalityInfo: new List<Legality> { Legality.StandartBanned }
            );
        }
    }
}
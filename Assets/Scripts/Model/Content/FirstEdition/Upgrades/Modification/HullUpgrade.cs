using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class HullUpgrade : GenericUpgrade
    {
        public HullUpgrade() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Hull Upgrade",
                UpgradeType.Modification,
                cost: 3,
                addHull: 1
            );
        }
    }
}
using Upgrade;
using Ship;

namespace UpgradesList.FirstEdition
{
    public class ShieldUpgrade : GenericUpgrade
    {
        public ShieldUpgrade() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Shield Upgrade",
                UpgradeType.Modification,
                cost: 4,
                addShields: 1
            );
        }
    }
}
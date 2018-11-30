using Upgrade;
using Ship;

namespace UpgradesList.FirstEdition
{
    public class R2D6 : GenericUpgrade
    {
        public R2D6() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "R2-D6",
                UpgradeType.Astromech,
                cost: 1,
                isLimited: true,
                addSlot: new UpgradeSlot(UpgradeType.Elite)
            );
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            if (ship.PilotInfo.Initiative <= 2)
            {
                return false;
            }

            if (ship.PilotInfo.ExtraUpgrades.Contains(UpgradeType.Elite))
            {
                return false;
            }

            return true;
        }
    }
}
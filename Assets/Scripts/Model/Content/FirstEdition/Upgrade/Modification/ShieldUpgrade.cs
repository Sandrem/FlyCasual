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
                cost: 4
            );
        }

        public override void PreAttachToShip(GenericShip host)
        {
            base.PreAttachToShip(host);

            Host.ShipInfo.Shields++;
        }

        public override void PreDettachFromShip()
        {
            base.PreDettachFromShip();

            Host.ShipInfo.Shields--;
        }
    }
}
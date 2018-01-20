using Ship;
using Upgrade;

namespace UpgradesList
{
    public class ShieldUpgrade : GenericUpgrade
    {
        public ShieldUpgrade() : base()
        {
            Type = UpgradeType.Modification;
            Name = "Shield Upgrade";
            Cost = 4;
        }

        public override void PreAttachToShip(GenericShip host)
        {
            base.PreAttachToShip(host);

            Host.MaxShields++;
        }

        public override void PreDettachFromShip()
        {
            base.PreDettachFromShip();

            Host.MaxShields--;
        }
    }
}

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

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);
            host.ChangeShieldBy(1);
            host.AfterGetMaxShields += IncreaseMaxShields;
        }

        private void IncreaseMaxShields(ref int maxShields)
        {
            maxShields++;
        }
    }
}

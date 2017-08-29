using Ship;
using Upgrade;

namespace UpgradesList
{
    public class ShieldUpgrade : GenericUpgrade
    {
        public ShieldUpgrade() : base()
        {
            Type = UpgradeType.Modification;
            Name = ShortName = "Shield Upgrade";
            ImageUrl = "https://vignette1.wikia.nocookie.net/xwing-miniatures/images/d/df/Shield_Upgrade.png";
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

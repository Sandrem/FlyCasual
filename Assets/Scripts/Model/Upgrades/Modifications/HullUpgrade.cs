using Ship;
using Upgrade;

namespace UpgradesList
{
    public class HullUpgrade : GenericUpgrade
    {
        public HullUpgrade() : base()
        {

            Type = UpgradeType.Modification;
            Name = ShortName = "Hull Upgrade";
            ImageUrl = "https://vignette1.wikia.nocookie.net/xwing-miniatures/images/2/21/Hull_Modification.png";
            Cost = 3;
        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);
            host.ChangeHullBy(1);
            host.AfterGetMaxHull += IncreaseMaxHull;
        }

        private void IncreaseMaxHull(ref int maxHull)
        {
            maxHull++;
        }
    }
}

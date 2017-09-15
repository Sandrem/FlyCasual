using Ship;
using Ship.AWing;
using Upgrade;

namespace UpgradesList
{
    public class ChardaanRefit : GenericUpgrade
    {
        public ChardaanRefit() : base()
        {
            Type = UpgradeType.Missile;
            Name = ShortName = "Chardaan Refit";
            ImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures/images/0/0d/Chardaan-refit.png";
            Cost = -2;
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is AWing;
        }
    }
}

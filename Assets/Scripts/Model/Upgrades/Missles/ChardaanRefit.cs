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
            Cost = -2;
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is AWing;
        }
    }
}

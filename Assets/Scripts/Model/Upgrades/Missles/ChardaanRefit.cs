using Ship;
using Ship.AWing;
using Upgrade;

namespace UpgradesList
{
    public class ChardaanRefit : GenericUpgrade
    {
        public ChardaanRefit() : base()
        {
            Types.Add(UpgradeType.Missile);
            Name = "Chardaan Refit";
            Cost = -2;
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is AWing;
        }
    }
}

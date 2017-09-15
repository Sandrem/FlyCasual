using Ship;
using Ship.YWing;
using Upgrade;
using System.Collections.Generic;

namespace UpgradesList
{
    public class BombLoadout : GenericUpgradeSlotUpgrade
    {
        public BombLoadout() : base()
        {
            Type = UpgradeType.Torpedo;
            Name = ShortName = "Bomb Loadout";
            ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/upgrades/Torpedo/bomb-loadout.png";
            Cost = 0;
            Limited = true;
            AddedSlots = new List<UpgradeSlot>
            {
                new UpgradeSlot(UpgradeType.Bomb)
            };
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is YWing;
        }
    }
}

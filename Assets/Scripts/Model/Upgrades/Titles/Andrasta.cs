using Ship;
using Ship.Firespray31;
using Upgrade;
using System.Collections.Generic;

namespace UpgradesList
{
    public class Andrasta : GenericUpgradeSlotUpgrade
    {
        public Andrasta() : base()
        {
            Type = UpgradeType.Title;
            Name = ShortName = "Andrasta";
            ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/upgrades/Title/andrasta.png";
            Cost = 0;
            isUnique = true;
            AddedSlots = new List<UpgradeSlot>
            {
                new UpgradeSlot(UpgradeType.Bomb),
                new UpgradeSlot(UpgradeType.Bomb)
            };
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is Firespray31;
        }
    }
}

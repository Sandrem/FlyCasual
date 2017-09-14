using Ship;
using Ship.Firespray31;
using Upgrade;
using System.Collections.Generic;

namespace UpgradesList
{
    public class SlaveI : GenericUpgradeSlotUpgrade
    {
        public SlaveI() : base()
        {
            Type = UpgradeType.Title;
            Name = ShortName = "Slave I";
            ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/upgrades/Title/slave-i.png";
            Cost = 0;
            isUnique = true;
            AddedSlots = new List<UpgradeSlot>
            {
                new UpgradeSlot(UpgradeType.Torpedoes)
            };
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is Firespray31;
        }
    }
}

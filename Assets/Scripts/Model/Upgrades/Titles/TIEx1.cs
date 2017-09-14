using Ship;
using Ship.TIEAdvanced;
using Upgrade;
using System.Collections.Generic;

namespace UpgradesList
{
    public class TIEx1 : GenericUpgradeSlotUpgrade
    {
        public TIEx1() : base()
        {
            Type = UpgradeType.Title;
            Name = ShortName = "TIE/x1";
            ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/upgrades/Title/tie-x1.png";
            Cost = 0;
            AddedSlots = new List<UpgradeSlot>
            {
                new UpgradeSlot(UpgradeType.System) { CostDecrease = 4 }
            };
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is TIEAdvanced;
        }
    }
}

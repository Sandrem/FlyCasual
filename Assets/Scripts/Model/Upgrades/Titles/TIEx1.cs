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
            Types.Add(UpgradeType.Title);
            Name = "TIE/x1";
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

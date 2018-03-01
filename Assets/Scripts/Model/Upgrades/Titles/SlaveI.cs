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
            Types.Add(UpgradeType.Title);
            Name = "Slave I";
            Cost = 0;
            isUnique = true;
            AddedSlots = new List<UpgradeSlot>
            {
                new UpgradeSlot(UpgradeType.Torpedo)
            };
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is Firespray31;
        }
    }
}

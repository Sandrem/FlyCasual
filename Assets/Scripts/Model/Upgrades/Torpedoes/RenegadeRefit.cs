using Ship;
using Ship.UWing;
using Ship.XWing;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace UpgradesList
{

    public class RenegadeRefit : GenericUpgradeSlotUpgrade
    {
        public RenegadeRefit() : base()
        {
            Types.Add(UpgradeType.Torpedo);

            Name = "Renegade Refit";
            Cost = -2;
            AddedSlots = new List<UpgradeSlot>
            {
                new UpgradeSlot(UpgradeType.Modification) { MustBeDifferent = true }
            };

            CostReductionByType = new Dictionary<UpgradeType, int>()
            {
                { UpgradeType.Elite, 1 }
            };
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is XWing || ship is UWing;
        }
    }
}


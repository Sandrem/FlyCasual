using Ship;
using Upgrade;
using System.Collections.Generic;
using Ship.ScurrgH6Bomber;

namespace UpgradesList
{
    public class Havoc : GenericUpgradeSlotUpgrade
    {
        public Havoc() : base()
        {
            Types.Add(UpgradeType.Title);
            Name = "Havoc";
            Cost = 0;
            AddedSlots = new List<UpgradeSlot>
            {
                new UpgradeSlot(UpgradeType.System) {  },
                new UpgradeSlot(UpgradeType.SalvagedAstromech) { MustBeUnique = true }
            };
            ForbiddenSlots = new List<UpgradeType>
            {
                UpgradeType.Crew
            };
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is ScurrgH6Bomber;
        }
    }
}

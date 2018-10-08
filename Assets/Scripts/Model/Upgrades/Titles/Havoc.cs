using Ship;
using Upgrade;
using System.Collections.Generic;
using Ship.ScurrgH6Bomber;
using RuleSets;

namespace UpgradesList
{
    public class Havoc : GenericUpgradeSlotUpgrade, ISecondEditionUpgrade
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

        public void AdaptUpgradeToSecondEdition()
        {
            Cost = 4;
            isUnique = true;
            AddedSlots = new List<UpgradeSlot>
            {
                new UpgradeSlot(UpgradeType.System) {  },
                new UpgradeSlot(UpgradeType.Astromech)
            };

            SEImageNumber = 147;
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is ScurrgH6Bomber;
        }
    }
}

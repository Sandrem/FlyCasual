using Ship;
using Ship.Firespray31;
using Upgrade;
using System.Collections.Generic;
using RuleSets;
using ActionsList;
using Abilities;

namespace UpgradesList
{
    public class Andrasta : GenericUpgradeSlotUpgrade, ISecondEditionUpgrade
    {
        public Andrasta() : base()
        {
            Types.Add(UpgradeType.Title);
            Name = "Andrasta";
            Cost = 0;
            isUnique = true;
            AddedSlots = new List<UpgradeSlot>
            {
                new UpgradeSlot(UpgradeType.Bomb),
                new UpgradeSlot(UpgradeType.Bomb)
            };
        }

        public void AdaptUpgradeToSecondEdition()
        {
            Cost = 6;
            AddedSlots = new List<UpgradeSlot>
            {
                new UpgradeSlot(UpgradeType.Bomb)
            };
            UpgradeAbilities.Add(new GenericActionBarAbility<ReloadAction>(false, null));
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is Firespray31;
        }
    }
}

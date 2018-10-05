using Ship;
using Ship.Firespray31;
using Upgrade;
using System.Collections.Generic;
using RuleSets;
using Abilities;

namespace UpgradesList
{
    public class SlaveI : GenericUpgradeSlotUpgrade, ISecondEditionUpgrade
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

        public void AdaptUpgradeToSecondEdition()
        {
            Cost = 5;
            UpgradeAbilities.Add(new BobaFettEmpireAbility());

            SEImageNumber = 154;
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is Firespray31;
        }
    }
}

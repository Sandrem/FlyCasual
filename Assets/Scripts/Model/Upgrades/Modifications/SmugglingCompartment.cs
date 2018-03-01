using Ship;
using Upgrade;
using System.Collections.Generic;


namespace UpgradesList
{
    public class SmugglingCompartment : GenericUpgradeSlotUpgrade
    {
        public SmugglingCompartment() : base()
        {
            Types.Add(UpgradeType.Modification);
            Name = "Smuggling Compartment";
            Cost = 0;
            isLimited = true;

            AddedSlots = new List<UpgradeSlot>
            {
                new UpgradeSlot(UpgradeType.Illicit) {  },
                new UpgradeSlot(UpgradeType.Modification) { MaxCost = 3 }
            };            
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ((ship is Ship.YT1300.YT1300) || (ship is Ship.YT2400.YT2400));
        }
    }
}

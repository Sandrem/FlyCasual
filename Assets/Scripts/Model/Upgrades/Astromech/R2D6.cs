using Ship;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace UpgradesList
{

    public class R2D6 : GenericUpgradeSlotUpgrade
    {
        public R2D6() : base()
        {
            Types.Add(UpgradeType.Astromech);
            Name = "R2-D6";
            isUnique = true;
            Cost = 1;

            AddedSlots = new List<UpgradeSlot>
            {
                new UpgradeSlot(UpgradeType.Elite)
            };
        }        

        public override bool IsAllowedForShip(GenericShip ship)
        {
            if (ship.PilotSkill <= 2)
            {
                return false;
            }

            if (ship.PrintedUpgradeIcons.Any(upgrade => upgrade == UpgradeType.Elite))
            {
                return false;
            }

            return true;
        }
    }
}
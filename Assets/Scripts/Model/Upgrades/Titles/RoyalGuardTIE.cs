using Ship;
using Ship.TIEInterceptor;
using Upgrade;
using System.Collections.Generic;

namespace UpgradesList
{
    public class RoyalGuardTIE : GenericUpgradeSlotUpgrade
    {
        public RoyalGuardTIE() : base()
        {
            Type = UpgradeType.Title;
            Name = ShortName = "Royal Guard TIE";
            ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/upgrades/Title/royal-guard-tie.png";
            Cost = 0;
            AddedSlots = new List<UpgradeSlot>
            {
                new UpgradeSlot(UpgradeType.Modification) { MustBeDifferent = true }
            };
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ((ship is TIEInterceptor) && (ship.PilotSkill > 4));
        }
    }
}

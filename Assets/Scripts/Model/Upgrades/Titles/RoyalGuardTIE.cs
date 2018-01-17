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
            Types.Add(UpgradeType.Title);
            Name = "Royal Guard TIE";
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

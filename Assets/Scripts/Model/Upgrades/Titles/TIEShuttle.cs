using Ship;
using Ship.TIEBomber;
using Upgrade;
using System.Collections.Generic;

namespace UpgradesList
{
    public class TIEShuttle : GenericUpgradeSlotUpgrade
    {
        public TIEShuttle() : base()
        {
            Type = UpgradeType.Title;
            Name = "TIE Shuttle";
            Cost = 0;
            AddedSlots = new List<UpgradeSlot>
            {
                new UpgradeSlot(UpgradeType.Crew) { MaxCost = 4 },
                new UpgradeSlot(UpgradeType.Crew) { MaxCost = 4 }
            };
            ForbiddenSlots = new List<UpgradeType>
            {
                UpgradeType.Torpedo,
                UpgradeType.Missile,
                UpgradeType.Bomb
            };
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is TIEBomber;
        }
    }
}

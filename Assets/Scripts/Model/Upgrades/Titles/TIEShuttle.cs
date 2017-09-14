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
            Name = ShortName = "TIE Shuttle";
            ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/upgrades/Title/tie-shuttle.png";
            Cost = 0;
            AddedSlots = new List<UpgradeSlot>
            {
                new UpgradeSlot(UpgradeType.Crew) { MaxCost = 4 },
                new UpgradeSlot(UpgradeType.Crew) { MaxCost = 4 }
            };
            ForbiddenSlots = new List<UpgradeType>
            {
                UpgradeType.Torpedoes,
                UpgradeType.Missiles,
                UpgradeType.Bomb
            };
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is TIEBomber;
        }
    }
}

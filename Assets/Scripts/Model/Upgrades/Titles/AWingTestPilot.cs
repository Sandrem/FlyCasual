using Ship;
using Ship.AWing;
using Upgrade;
using UnityEngine;
using System.Collections.Generic;

namespace UpgradesList
{
    public class AWingTestPilot : GenericUpgradeSlotUpgrade
    {
        public AWingTestPilot() : base()
        {
            Type = UpgradeType.Title;
            Name = ShortName = "A-Wing Test Pilot";
            ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/upgrades/Title/a-wing-test-pilot.png";
            Cost = 0;
            AddedSlots = new List<UpgradeSlot>
            {
                new UpgradeSlot(UpgradeType.Elite) { MustBeDifferent = true }
            };
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ((ship is AWing) && (ship.PilotSkill > 1));
        }
    }
}

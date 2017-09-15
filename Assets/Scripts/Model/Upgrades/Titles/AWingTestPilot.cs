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

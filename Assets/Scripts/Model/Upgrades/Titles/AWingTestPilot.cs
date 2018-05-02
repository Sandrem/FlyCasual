using Ship;
using Ship.AWing;
using Upgrade;
using UnityEngine;
using System.Collections.Generic;
using SquadBuilderNS;

namespace UpgradesList
{
    public class AWingTestPilot : GenericUpgradeSlotUpgrade
    {
        public AWingTestPilot() : base()
        {
            Types.Add(UpgradeType.Title);
            Name = "A-Wing Test Pilot";
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

        public override bool IsAllowedForSquadBuilderPostCheck(SquadList squadList)
        {
            bool result = Host.PilotSkill > 1;
            if (!result) Messages.ShowError("You cannot equip \"A-Wing Test Pilot\" if pilot's skill is \"1\"");
            return result;
        }
    }
}

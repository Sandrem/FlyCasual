using Ship;
using Ship.TIEInterceptor;
using Upgrade;
using System.Collections.Generic;
using SquadBuilderNS;

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

        public override bool IsAllowedForSquadBuilderPostCheck(SquadList squadList)
        {
            bool result = Host.PilotSkill > 4;
            if (!result) Messages.ShowError("You cannot equip \"Royal Guard TIE\" if pilot's skill is \"4\" or lower");
            return result;
        }
    }
}

using Ship;
using SquadBuilderNS;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class AWingTestPilot : GenericUpgrade
    {
        public AWingTestPilot() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "A-Wing Test Pilot",
                UpgradeType.Title,
                cost: 0,
                restriction: new ShipRestriction(typeof(Ship.FirstEdition.AWing.AWing)),
                addSlot: new UpgradeSlot(UpgradeType.Talent) { MustBeDifferent = true }
            );
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.PilotInfo.Initiative > 1;
        }

        public override bool IsAllowedForSquadBuilderPostCheck(SquadList squadList)
        {
            bool result = Host.PilotInfo.Initiative > 1;
            if (!result) Messages.ShowError("You cannot equip \"A-Wing Test Pilot\" if pilot's skill is \"1\" or lower");
            return result;
        }
    }
}
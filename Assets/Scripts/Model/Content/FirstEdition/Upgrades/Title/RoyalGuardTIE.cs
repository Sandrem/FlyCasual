using Ship;
using SquadBuilderNS;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class RoyalGuardTIE : GenericUpgrade
    {
        public RoyalGuardTIE() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Royal Guard TIE",
                UpgradeType.Title,
                cost: 0,
                restriction: new ShipRestriction(typeof(Ship.FirstEdition.TIEInterceptor.TIEInterceptor)),
                addSlot: new UpgradeSlot(UpgradeType.Modification) { MustBeDifferent = true }
            );
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.PilotInfo.Initiative > 4;
        }

        public override bool IsAllowedForSquadBuilderPostCheck(SquadList squadList)
        {
            bool result = HostShip.PilotInfo.Initiative > 4;
            if (!result) Messages.ShowError("You cannot equip \"Royal Guard TIE\" if pilot's skill is \"4\" or lower");
            return result;
        }
    }
}
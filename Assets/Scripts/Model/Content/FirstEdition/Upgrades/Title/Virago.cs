using Ship;
using SquadBuilderNS;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class Virago : GenericUpgrade
    {
        public Virago() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Virago",
                UpgradeType.Title,
                cost: 1,
                isLimited: true,
                restriction: new ShipRestriction(typeof(Ship.FirstEdition.StarViper.StarViper)),
                addSlots: new List<UpgradeSlot>()
                {
                    new UpgradeSlot(UpgradeType.System),
                    new UpgradeSlot(UpgradeType.Illicit)
                }
            );
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.PilotInfo.Initiative > 3;
        }

        public override bool IsAllowedForSquadBuilderPostCheck(SquadList squadList)
        {
            bool result = HostShip.PilotInfo.Initiative > 3;
            if (!result) Messages.ShowError("You cannot equip \"Virago\" if the pilot's skill is \"3\" or lower");
            return result;
        }
    }
}
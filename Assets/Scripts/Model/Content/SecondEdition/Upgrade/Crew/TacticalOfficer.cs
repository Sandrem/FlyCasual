using Ship;
using Upgrade;
using System.Collections.Generic;
using Abilities;
using ActionsList;

namespace UpgradesList.SecondEdition
{
    public class TacticalOfficer : GenericUpgrade
    {
        public TacticalOfficer() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Tactical Officer",
                UpgradeType.Crew,
                cost: 2,
                seImageNumber: 48
            );

            UpgradeAbilities.Add(new GenericActionBarAbility<CoordinateAction>());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.ActionBar.HasAction(typeof(CoordinateAction), isRed: true);
        }
    }
}
using Ship;
using Upgrade;
using System.Collections.Generic;
using Abilities;
using ActionsList;
using Actions;

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
                restriction: new ActionBarRestriction(new ActionInfo(typeof(CoordinateAction), ActionColor.Red)),
                seImageNumber: 48
            );

            UpgradeAbilities.Add(new GenericActionBarAbility<CoordinateAction>());
        }
    }
}
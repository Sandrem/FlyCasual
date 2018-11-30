using Ship;
using Upgrade;
using System.Collections.Generic;
using Abilities;
using ActionsList;

namespace UpgradesList.FirstEdition
{
    public class TacticalOfficer : GenericUpgrade
    {
        public TacticalOfficer() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Tactical Officer",
                UpgradeType.Crew,
                cost: 2,
                restriction: new FactionRestriction(Faction.Imperial)
            );

            UpgradeAbilities.Add(new GenericActionBarAbility<CoordinateAction>());
        }        
    }
}
using Ship;
using Upgrade;
using UnityEngine;

namespace UpgradesList.FirstEdition
{
    public class FleetOfficer : GenericUpgrade
    {
        public FleetOfficer() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Fleet Officer",
                UpgradeType.Crew,
                cost: 3
            );

            Avatar = new AvatarInfo(Faction.Imperial, new Vector2(19, 1));
        }        
    }
}
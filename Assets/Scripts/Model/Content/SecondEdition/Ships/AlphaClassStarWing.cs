using System.Collections;
using System.Collections.Generic;
using Movement;
using ActionsList;
using Actions;
using Arcs;
using Upgrade;
using Ship;
using Bombs;

namespace Ship
{
    namespace SecondEdition.AlphaClassStarWing
    {
        public class AlphaClassStarWing : FirstEdition.AlphaClassStarWing.AlphaClassStarWing
        {
            public AlphaClassStarWing() : base()
            {
                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.System);
                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Configuration);

                IconicPilots[Faction.Imperial] = typeof(MajorVynder);

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/c/ce/Maneuver_alpha-class_star_wing.png";
            }
        }
    }
}
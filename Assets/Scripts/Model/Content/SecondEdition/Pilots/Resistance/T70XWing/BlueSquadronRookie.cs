using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.T70XWing
    {
        public class BlueSquadronRookie : T70XWing
        {
            public BlueSquadronRookie() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Blue Squadron Rookie",
                    "",
                    Faction.Resistance,
                    1,
                    5,
                    4,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Tech,
                        UpgradeType.Astromech,
                        UpgradeType.Modification,
                        UpgradeType.Configuration
                    },
                    tags: new List<Tags>
                    {
                        Tags.XWing
                    }
                );

                ImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/2/24/Swz25_blue-sqd_a1.png";
            }
        }
    }
}

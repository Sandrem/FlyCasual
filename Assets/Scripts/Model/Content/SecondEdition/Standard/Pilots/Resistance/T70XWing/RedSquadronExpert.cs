using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.T70XWing
    {
        public class RedSquadronExpert : T70XWing
        {
            public RedSquadronExpert() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Red Squadron Expert",
                    "",
                    Faction.Resistance,
                    3,
                    5,
                    2,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent
                    },
                    tags: new List<Tags>
                    {
                        Tags.XWing
                    },
                    skinName: "Red"
                );

                ImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/a/a9/Swz25_red-sqd_a1.png";
            }
        }
    }
}

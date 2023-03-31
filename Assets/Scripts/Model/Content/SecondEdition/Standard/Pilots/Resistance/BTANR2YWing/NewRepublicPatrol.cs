using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.BTANR2YWing
    {
        public class NewRepublicPatrol : BTANR2YWing
        {
            public NewRepublicPatrol() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "New Republic Patrol",
                    "",
                    Faction.Resistance,
                    3,
                    4,
                    7,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent
                    },
                    tags: new List<Tags>
                    {
                        Tags.YWing
                    },
                    skinName: "Blue"
                );

                ImageUrl = "https://i.imgur.com/7zFI7ZH.png";
            }
        }
    }
}

using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.BTANR2YWing
    {
        public class KijimiSpiceRunner : BTANR2YWing
        {
            public KijimiSpiceRunner() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Kijimi Spice Runner",
                    "",
                    Faction.Resistance,
                    2,
                    4,
                    4,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Illicit
                    },
                    tags: new List<Tags>
                    {
                        Tags.YWing
                    },
                    skinName: "Red"
                );

                ImageUrl = "https://static.wikia.nocookie.net/xwing-miniatures-second-edition/images/0/07/KijimiSpiceRunner.png";
            }
        }
    }
}

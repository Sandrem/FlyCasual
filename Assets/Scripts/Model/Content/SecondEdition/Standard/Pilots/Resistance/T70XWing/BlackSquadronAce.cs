using Content;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.T70XWing
    {
        public class BlackSquadronAce : T70XWing
        {
            public BlackSquadronAce() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Black Squadron Ace",
                    "",
                    Faction.Resistance,
                    4,
                    5,
                    10,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent
                    },
                    tags: new List<Tags>
                    {
                        Tags.XWing
                    },
                    skinName: "Black One"
                );

                PilotNameCanonical = "blacksquadronace-t70xwing";

                ImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/a/a7/Swz25_black-sqd-ace_a1.png";
            }
        }
    }
}

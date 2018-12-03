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
                PilotInfo = new PilotCardInfo(
                    "Black Squadron Ace",
                    4,
                    50,
                    extraUpgradeIcon: UpgradeType.Talent //,
                    //seImageNumber: 11
                );

                ModelInfo.SkinName = "Black One";

                ImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/a/a7/Swz25_black-sqd-ace_a1.png";
            }
        }
    }
}

using System.Collections;
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
                PilotInfo = new PilotCardInfo(
                    "Red Squadron Expert",
                    3,
                    48,
                    extraUpgradeIcon: UpgradeType.Talent //,
                    //seImageNumber: 11
                );

                ModelInfo.SkinName = "Red";

                ImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/a/a9/Swz25_red-sqd_a1.png";
            }
        }
    }
}

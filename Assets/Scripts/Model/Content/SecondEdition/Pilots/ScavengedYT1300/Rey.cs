using BoardTools;
using System;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.ScavengedYT1300
    {
        public class Rey : ScavengedYT1300
        {
            public Rey() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Rey",
                    5,
                    80,
                    isLimited: true,
                    //abilityType: typeof(Abilities.SecondEdition.HanSoloRebelPilotAbility),
                    //charges: 1,
                    //regensCharges: true,
                    extraUpgradeIcon: UpgradeType.Talent //,
                    //seImageNumber: 69
                );

                ImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/5/58/Swz19_a2_reys-pilot.png";
            }
        }
    }
}
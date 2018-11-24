using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEInterceptor
    {
        public class SaberSquadronAce : TIEInterceptor
        {
            public SaberSquadronAce() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Saber Squadron Ace",
                    4,
                    40,
                    extraUpgradeIcon: UpgradeType.Elite,
                    seImageNumber: 105
                );

                ModelInfo.SkinName = "Red Stripes";
            }
        }
    }
}
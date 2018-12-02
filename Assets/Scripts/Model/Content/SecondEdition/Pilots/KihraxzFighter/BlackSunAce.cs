using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.KihraxzFighter
    {
        public class BlackSunAce : KihraxzFighter
        {
            public BlackSunAce() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Black Sun Ace",
                    3,
                    42,
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 195
                );

                ModelInfo.SkinName = "Black Sun (White)";
            }
        }
    }
}

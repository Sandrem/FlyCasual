

using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIESeBomber
    {
        public class SienarJeamusTestPilot : TIESeBomber
        {
            public SienarJeamusTestPilot() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Sienar-Jaemus Test Pilot",
                    2,
                    31
                );

                ImageUrl = "https://meta.listfortress.com/assets/pilots/sienarjaemustestpilot-4a274cc57c3edc4e4e7c2fa4ba5865ee58bf93163eae054390166bb9909d7a67.png";
            }
        }
    }
}

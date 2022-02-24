using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEWiWhisperModifiedInterceptor
    {
        public class RedFuryZealot : TIEWiWhisperModifiedInterceptor
        {
            public RedFuryZealot() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Red Fury Zealot",
                    2,
                    44,
                    extraUpgradeIcons: new List<UpgradeType>() { UpgradeType.Talent }
                );

                ImageUrl = "https://meta.listfortress.com/assets/pilots/redfuryzealot-f740cbbb920e579f88f70619b299e0e495411de02d130387ba3305071703c8ad.png";
            }
        }
    }
}

using Content;
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
                PilotInfo = new PilotCardInfo25
                (
                    "Red Fury Zealot",
                    "",
                    Faction.FirstOrder,
                    2,
                    4,
                    3,
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    }
                );

                ImageUrl = "https://meta.listfortress.com/assets/pilots/redfuryzealot-f740cbbb920e579f88f70619b299e0e495411de02d130387ba3305071703c8ad.png";
            }
        }
    }
}

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
                        UpgradeType.Talent,
                        UpgradeType.Tech,
                        UpgradeType.Tech,
                        UpgradeType.Configuration
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    }
                );

                ImageUrl = "https://i.imgur.com/BN4KmBV.png";
            }
        }
    }
}

using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEWiWhisperModifiedInterceptor
    {
        public class P709thLegionAce : TIEWiWhisperModifiedInterceptor
        {
            public P709thLegionAce() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "709th Legion Ace",
                    "",
                    Faction.FirstOrder,
                    4,
                    4,
                    10,
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Missile
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    }
                );

                ImageUrl = "https://i.imgur.com/0UHEp9m.png";
            }
        }
    }
}

using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEInterceptor
    {
        public class AlphaSquadronPilot : TIEInterceptor
        {
            public AlphaSquadronPilot() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Alpha Squadron Pilot",
                    "",
                    Faction.Imperial,
                    1,
                    4,
                    2,
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    seImageNumber: 106
                );
            }
        }
    }
}
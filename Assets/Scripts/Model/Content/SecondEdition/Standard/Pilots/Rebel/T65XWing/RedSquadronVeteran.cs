using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.T65XWing
    {
        public class RedSquadronVeteran : T65XWing
        {
            public RedSquadronVeteran() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Red Squadron Veteran",
                    "",
                    Faction.Rebel,
                    3,
                    5,
                    3,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Astromech
                    },
                    tags: new List<Tags>
                    {
                        Tags.XWing
                    },
                    seImageNumber: 10
                );
            }
        }
    }
}

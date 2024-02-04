using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEReaper
    {
        public class ScarifBasePilot : TIEReaper
        {
            public ScarifBasePilot() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Scarif Base Pilot",
                    "",
                    Faction.Imperial,
                    1,
                    4,
                    6,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Crew,
                        UpgradeType.Modification                        
                    },
                    seImageNumber: 116,
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    }
                );
            }
        }
    }
}

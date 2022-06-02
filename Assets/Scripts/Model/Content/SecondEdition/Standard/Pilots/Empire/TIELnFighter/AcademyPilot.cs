using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIELnFighter
    {
        public class AcademyPilot: TIELnFighter
        {
            public AcademyPilot() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Academy Pilot",
                    "",
                    Faction.Imperial,
                    1,
                    2,
                    3,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    seImageNumber: 92
                );
            }
        }
    }
}

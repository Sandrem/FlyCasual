using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.Z95AF4Headhunter
    {
        public class TalaSquadronPilot : Z95AF4Headhunter
        {
            public TalaSquadronPilot() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Tala Squadron Pilot",
                    "",
                    Faction.Rebel,
                    2,
                    3,
                    4,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Modification
                    },
                    seImageNumber: 29
                );
            }
        }
    }
}

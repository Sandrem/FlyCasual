using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.Z95AF4Headhunter
    {
        public class BanditSquadronPilot : Z95AF4Headhunter
        {
            public BanditSquadronPilot() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Bandit Squadron Pilot",
                    "",
                    Faction.Rebel,
                    1,
                    3,
                    5,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Missile,
                        UpgradeType.Modification
                    },
                    seImageNumber: 30
                );
            }
        }
    }
}

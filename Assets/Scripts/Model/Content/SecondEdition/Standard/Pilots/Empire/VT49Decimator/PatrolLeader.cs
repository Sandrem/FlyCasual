using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.VT49Decimator
    {
        public class PatrolLeader : VT49Decimator
        {
            public PatrolLeader() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Patrol Leader",
                    "",
                    Faction.Imperial,
                    2,
                    7,
                    12,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Torpedo,
                        UpgradeType.Crew,
                        UpgradeType.Gunner,
                        UpgradeType.Device,
                        UpgradeType.Modification
                    },
                    seImageNumber: 148
                );
            }
        }
    }
}

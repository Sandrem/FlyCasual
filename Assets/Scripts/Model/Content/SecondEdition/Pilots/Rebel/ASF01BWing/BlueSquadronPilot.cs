using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.ASF01BWing
    {
        public class BlueSquadronPilot : ASF01BWing
        {
            public BlueSquadronPilot() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Blue Squadron Pilot",
                    "",
                    Faction.Rebel,
                    2,
                    4,
                    4,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Sensor,
                        UpgradeType.Cannon,
                        UpgradeType.Cannon,
                        UpgradeType.Device,
                        UpgradeType.Configuration
                    },
                    tags: new List<Tags>
                    {
                        Tags.BWing
                    },
                    seImageNumber: 26
                );
            }
        }
    }
}

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
                    5,
                    6,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Device
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

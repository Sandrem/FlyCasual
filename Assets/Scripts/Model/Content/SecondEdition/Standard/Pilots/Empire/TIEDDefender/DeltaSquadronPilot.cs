using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEDDefender
    {
        public class DeltaSquadronPilot : TIEDDefender
        {
            public DeltaSquadronPilot() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Delta Squadron Pilot",
                    "",
                    Faction.Imperial,
                    1,
                    7,
                    4,
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Sensor
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    seImageNumber: 126
                );
            }
        }
    }
}

using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.LambdaClassT4AShuttle
    {
        public class OmicronGroupPilot : LambdaClassT4AShuttle
        {
            public OmicronGroupPilot() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Omicron Group Pilot",
                    "",
                    Faction.Imperial,
                    1,
                    5,
                    8,
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Sensor,
                        UpgradeType.Cannon,
                        UpgradeType.Modification
                    },
                    seImageNumber: 145,
                    legality: new List<Legality>() { Legality.ExtendedLegal }
                );
            }
        }
    }
}

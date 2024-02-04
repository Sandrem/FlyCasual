using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEPhPhantom
    {
        public class ImdaarTestPilot : TIEPhPhantom
        {
            public ImdaarTestPilot() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Imdaar Test Pilot",
                    "",
                    Faction.Imperial,
                    3,
                    5,
                    6,
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Sensor,
                        UpgradeType.Gunner,
                        UpgradeType.Modification
                    },
                    seImageNumber: 134,
                    legality: new List<Legality>() { Legality.ExtendedLegal }
                );
            }
        }
    }
}

using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIECaPunisher
    {
        public class CutlassSquadronPilot : TIECaPunisher
        {
            public CutlassSquadronPilot() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Cutlass Squadron Pilot",
                    "",
                    Faction.Imperial,
                    2,
                    5,
                    6,
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Sensor,
                        UpgradeType.Torpedo,
                        UpgradeType.Missile,
                        UpgradeType.Gunner,
                        UpgradeType.Device,
                        UpgradeType.Modification
                    },
                    seImageNumber: 141,
                    legality: new List<Legality>() { Legality.ExtendedLegal }
                );
            }
        }
    }
}

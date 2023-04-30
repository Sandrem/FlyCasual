using Content;
using Ship;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.Mg100StarFortress
    {
        public class CobaltSquadronBomber : Mg100StarFortress
        {
            public CobaltSquadronBomber() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Cobalt Squadron Bomber",
                    "",
                    Faction.Resistance,
                    1,
                    6,
                    11,
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Sensor,
                        UpgradeType.Tech,
                        UpgradeType.Gunner,
                        UpgradeType.Gunner,
                        UpgradeType.Device,
                        UpgradeType.Device,
                        UpgradeType.Modification
                    },
                    legality: new List<Legality>() { Legality.ExtendedLegal }
                );

                ModelInfo.SkinName = "Cobalt";
            }
        }
    }
}
using Content;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.BTLS8KWing
    {
        public class WardenSquadronPilot : BTLS8KWing
        {
            public WardenSquadronPilot() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Warden Squadron Pilot",
                    "",
                    Faction.Rebel,
                    2,
                    5,
                    7,
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Torpedo,
                        UpgradeType.Missile,
                        UpgradeType.Gunner,
                        UpgradeType.Device,
                        UpgradeType.Device
                    },
                    seImageNumber: 64,
                    legality: new List<Legality>() { Legality.ExtendedLegal }
                );
            }
        }
    }
}

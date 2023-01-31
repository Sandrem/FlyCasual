using Content;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.AuzituckGunship
    {
        public class KashyyykDefender : AuzituckGunship
        {
            public KashyyykDefender() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Kashyyyk Defender",
                    "",
                    Faction.Rebel,
                    1,
                    5,
                    6,
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Crew,
                        UpgradeType.Modification
                    },
                    seImageNumber: 33,
                    legality: new List<Legality>() { Legality.ExtendedLegal }
                );
            }
        }
    }
}

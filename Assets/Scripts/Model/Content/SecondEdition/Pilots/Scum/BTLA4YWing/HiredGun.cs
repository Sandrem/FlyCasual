using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.BTLA4YWing
    {
        public class HiredGun : BTLA4YWing
        {
            public HiredGun() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Hired Gun",
                    "",
                    Faction.Scum,
                    2,
                    4,
                    6,
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Turret,
                        UpgradeType.Torpedo,
                        UpgradeType.Missile,
                        UpgradeType.Device,
                        UpgradeType.Illicit
                    },
                    tags: new List<Tags>
                    {
                        Tags.YWing
                    },
                    seImageNumber: 167,
                    skinName: "Gray"
                );
            }
        }
    }
}

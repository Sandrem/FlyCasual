using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.BTLA4YWing
    {
        public class CrymorahGoon : BTLA4YWing
        {
            public CrymorahGoon() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Crymorah Goon",
                    "",
                    Faction.Scum,
                    1,
                    4,
                    4,
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Turret,
                        UpgradeType.Missile,
                        UpgradeType.Device,
                        UpgradeType.Illicit
                    },
                    tags: new List<Tags>
                    {
                        Tags.YWing
                    },
                    seImageNumber: 168,
                    skinName: "Brown"
                );
            }
        }
    }
}

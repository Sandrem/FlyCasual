using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.YV666LightFreighter
    {
        public class TrandoshanSlaver : YV666LightFreighter
        {
            public TrandoshanSlaver() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Trandoshan Slaver",
                    "",
                    Faction.Scum,
                    2,
                    6,
                    6,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Cannon,
                        UpgradeType.Missile,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Freighter
                    },
                    seImageNumber: 213
                );
            }
        }
    }
}
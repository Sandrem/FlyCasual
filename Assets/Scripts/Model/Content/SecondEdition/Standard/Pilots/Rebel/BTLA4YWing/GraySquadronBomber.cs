using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.BTLA4YWing
    {
        public class GraySquadronBomber : BTLA4YWing
        {
            public GraySquadronBomber() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Gray Squadron Bomber",
                    "",
                    Faction.Rebel,
                    2,
                    4,
                    8,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Device,
                        UpgradeType.Missile,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.YWing
                    },
                    seImageNumber: 18
                );
            }
        }
    }
}

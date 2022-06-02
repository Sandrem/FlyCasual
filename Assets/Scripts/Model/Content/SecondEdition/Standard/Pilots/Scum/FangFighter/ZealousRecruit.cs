using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.FangFighter
    {
        public class ZealousRecruit : FangFighter
        {
            public ZealousRecruit() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Zealous Recruit",
                    "",
                    Faction.Scum,
                    1,
                    5,
                    8,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Mandalorian
                    },
                    seImageNumber: 160
                );
            }
        }
    }
}
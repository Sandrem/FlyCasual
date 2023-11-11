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
                    4,
                    5,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Modification,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Mandalorian
                    },
                    seImageNumber: 160,
                    skinName: "Zealous Recruit"
                );
            }
        }
    }
}
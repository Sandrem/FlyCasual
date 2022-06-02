using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.T65XWing
    {
        public class CavernAngelsZealot : T65XWing
        {
            public CavernAngelsZealot() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Cavern Angels Zealot",
                    "",
                    Faction.Rebel,
                    1,
                    5,
                    4,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Astromech,
                        UpgradeType.Illicit
                    },
                    tags: new List<Tags>
                    {
                        Tags.Partisan,
                        Tags.XWing
                    },
                    seImageNumber: 12,
                    skinName: "Partisan"
                );
            }
        }
    }
}

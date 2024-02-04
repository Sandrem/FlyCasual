using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.Z95AF4Headhunter
    {
        public class BlackSunSoldier : Z95AF4Headhunter
        {
            public BlackSunSoldier() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Black Sun Soldier",
                    "",
                    Faction.Scum,
                    3,
                    3,
                    4,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Modification,
                        UpgradeType.Illicit
                    },
                    seImageNumber: 172,
                    skinName: "Black Sun"
                );
            }
        }
    }
}

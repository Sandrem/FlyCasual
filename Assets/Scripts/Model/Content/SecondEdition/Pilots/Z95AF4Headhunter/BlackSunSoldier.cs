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
                PilotInfo = new PilotCardInfo(
                    "Black Sun Soldier",
                    3,
                    27,
                    extraUpgradeIcons: new List<UpgradeType>(){ UpgradeType.Elite, UpgradeType.Illicit },
                    factionOverride: Faction.Scum,
                    seImageNumber: 172
                );
            }
        }
    }
}

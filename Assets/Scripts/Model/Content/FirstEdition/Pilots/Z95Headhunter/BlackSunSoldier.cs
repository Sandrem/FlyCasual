using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.Z95Headhunter
    {
        public class BlackSunSoldier : Z95Headhunter
        {
            public BlackSunSoldier() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Black Sun Soldier",
                    3,
                    13,
                    extraUpgradeIcon: UpgradeType.Illicit,
                    factionOverride: Faction.Scum
                );
            }
        }
    }
}

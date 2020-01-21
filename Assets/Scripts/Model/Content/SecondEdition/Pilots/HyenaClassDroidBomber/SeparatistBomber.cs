using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.HyenaClassDroidBomber
{
    public class SeparatistBomber : HyenaClassDroidBomber
    {
        public SeparatistBomber()
        {
            PilotInfo = new PilotCardInfo(
                "Separatist Bomber",
                3,
                28,
                extraUpgradeIcons: new List<UpgradeType> { UpgradeType.Torpedo, UpgradeType.Missile, UpgradeType.Device }
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/5d/88/5d88fdec-55d6-4ea4-9fa1-af79d0ca5fcd/swz41_separatist-bomber.png";
        }
    }
}
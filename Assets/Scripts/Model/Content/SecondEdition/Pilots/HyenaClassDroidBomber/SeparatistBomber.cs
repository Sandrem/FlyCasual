using Arcs;
using System;
using System.Collections.Generic;
using System.Linq;
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
                extraUpgradeIcon: UpgradeType.Talent
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/5d/88/5d88fdec-55d6-4ea4-9fa1-af79d0ca5fcd/swz41_separatist-bomber.png";
        }
    }
}
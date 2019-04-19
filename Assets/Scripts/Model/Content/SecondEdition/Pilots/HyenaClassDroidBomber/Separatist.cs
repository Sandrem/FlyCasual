using Arcs;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.HyenaClassDroidBomber
{
    public class Separatist : HyenaClassDroidBomber
    {
        public Separatist()
        {
            PilotInfo = new PilotCardInfo(
                "Separatist???",
                3,
                28,
                extraUpgradeIcon: UpgradeType.Talent
            );
        }
    }
}
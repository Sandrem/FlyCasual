﻿using System.Collections;
using System.Collections.Generic;

namespace Ship
{
    namespace SecondEdition.YT2400LightFreighter
    {
        public class WildSpaceFringer : YT2400LightFreighter
        {
            public WildSpaceFringer() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Wild Space Fringer",
                    1,
                    77,
                    extraUpgradeIcon: Upgrade.UpgradeType.Crew,
                    seImageNumber: 79
                );
            }
        }
    }
}

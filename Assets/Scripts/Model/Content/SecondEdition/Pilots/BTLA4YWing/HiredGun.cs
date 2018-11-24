﻿using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.BTLA4YWing
    {
        public class HiredGun : BTLA4YWing
        {
            public HiredGun() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Hired Gun",
                    2,
                    34,
                    extraUpgradeIcons: new List<UpgradeType>() { UpgradeType.Elite, UpgradeType.Illicit },
                    factionOverride: Faction.Scum,
                    seImageNumber: 167
                );

                ModelInfo.SkinName = "Gray";
            }
        }
    }
}
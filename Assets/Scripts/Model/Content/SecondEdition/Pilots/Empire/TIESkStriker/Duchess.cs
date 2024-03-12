﻿using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIESkStriker
    {
        public class Duchess : TIESkStriker
        {
            public Duchess() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "\"Duchess\"",
                    "Urbane Ace",
                    Faction.Imperial,
                    5,
                    5,
                    18,
                    isLimited: true,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Talent,
                        UpgradeType.Gunner,
                        UpgradeType.Device,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    seImageNumber: 117
                );
            }
        }
    }
}
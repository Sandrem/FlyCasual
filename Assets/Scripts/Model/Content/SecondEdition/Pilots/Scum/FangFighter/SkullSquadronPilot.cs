﻿using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.FangFighter
    {
        public class SkullSquadronPilot : FangFighter
        {
            public SkullSquadronPilot() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Skull Squadron Pilot",
                    "",
                    Faction.Scum,
                    4,
                    4,
                    6,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Torpedo,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Mandalorian
                    },
                    seImageNumber: 159,
                    skinName: "Skull Squadron"
                );
            }
        }
    }
}
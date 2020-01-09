﻿using ActionsList;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.AttackShuttle
    {
        public class SabineWren : AttackShuttle
        {
            public SabineWren() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Sabine Wren",
                    3,
                    41,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.SabineWrenPilotAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 35
                );
            }
        }
    }
}
﻿using ActionsList;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIELnFighter
    {
        public class SabineWren : TIELnFighter
        {
            public SabineWren() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Sabine Wren",
                    3,
                    28,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.SabineWrenPilotAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    factionOverride: Faction.Rebel,
                    seImageNumber: 47
                );

                ModelInfo.ModelName = "TIE Fighter Rebel";
                ModelInfo.SkinName = "Rebel";
            }
        }
    }
}

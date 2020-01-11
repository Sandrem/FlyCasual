﻿using Upgrade;

namespace Ship
{
    namespace SecondEdition.AggressorAssaultFighter
    {
        public class IG88B : AggressorAssaultFighter
        {
            public IG88B() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "IG-88B",
                    4,
                    63,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.IG88BAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 198
                );
            }
        }
    }
}
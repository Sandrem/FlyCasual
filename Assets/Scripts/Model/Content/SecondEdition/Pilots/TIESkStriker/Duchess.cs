﻿using Ship;
using SubPhases;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIESkStriker
    {
        public class Duchess : TIESkStriker
        {
            public Duchess() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Duchess\"",
                    5,
                    42,
                    limited: 1,
                    abilityType: typeof(Abilities.SecondEdition.CountdownAbility),
                    extraUpgradeIcon: UpgradeType.Elite,
                    seImageNumber: 117
                );
            }
        }
    }
}
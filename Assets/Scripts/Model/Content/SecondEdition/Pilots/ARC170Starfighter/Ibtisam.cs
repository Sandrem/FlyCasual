﻿using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.ARC170Starfighter
    {
        public class Ibtisam : ARC170Starfighter
        {
            public Ibtisam() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Ibtisam",
                    3,
                    50,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.BraylenStrammAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 68
                );
            }
        }
    }
}
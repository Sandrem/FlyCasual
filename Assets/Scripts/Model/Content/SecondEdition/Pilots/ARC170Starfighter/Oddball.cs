﻿using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.ARC170Starfighter
    {
        public class OddBall : ARC170Starfighter
        {
            public OddBall() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Odd Ball\"",
                    5,
                    51,
                    isLimited: true,
                    factionOverride: Faction.Republic,
                    abilityType: typeof(Abilities.SecondEdition.OddBallAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );

                ModelInfo.SkinName = "Red";

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/ff/29/ff29970e-5ed7-416d-b5da-3918e226b3dc/swz33_odd-ball.png";
            }
        }
    }
}

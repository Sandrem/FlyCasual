﻿using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEInterceptor
    {
        public class VultSkerris : TIEInterceptor
        {
            public VultSkerris() : base()
            {
                RequiredMods = new List<Type>() { typeof(Mods.ModsList.UnreleasedContentMod) };

                PilotInfo = new PilotCardInfo(
                    "Vult Skerris",
                    5,
                    45,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.VultSkerrisDefenderAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    abilityText: " Action: Recover 1 charge take 1 strain. Before you engage you spend 1 charge to perform an action.",
                    charges: 1,
                    regensCharges: -1
                );

                PilotNameCanonical = "vultskerris-tieinterceptor";

                ModelInfo.SkinName = "Vult Skerris";
            }
        }
    }
}
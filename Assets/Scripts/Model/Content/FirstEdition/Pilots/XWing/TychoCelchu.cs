﻿using System.Collections;
using System.Collections.Generic;
using Mods.ModsList;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.XWing
    {
        public class TychoCelchu : XWing
        {
            public TychoCelchu() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Tycho Celchu",
                    8,
                    28,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.TychoCelchuAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );

                RequiredMods.Add(typeof(MyOtherRideIsMod));
                ImageUrl = "https://i.imgur.com/imayMBg.png";
            }
        }
    }
}
﻿using Mods.ModsList;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.BWing
    {
        public class BraylenStramm : BWing
        {
            public BraylenStramm() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Braylen Stramm",
                    3,
                    24,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.BraylenStrammAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );

                RequiredMods.Add(typeof(MyOtherRideIsMod));
                ImageUrl = "https://i.imgur.com/V6m7JN9.png";

                ModelInfo.SkinName = "Dark Blue";
            }
        }
    }
}

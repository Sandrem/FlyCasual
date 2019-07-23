using Mods.ModsList;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEDDefender
    {
        public class DarthVader : TIEDDefender
        {
            public DarthVader() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Darth Vader",
                    6,
                    125,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.DarthVaderAbility),
                    force: 3,
                    extraUpgradeIcon: UpgradeType.Force
                );

                RequiredMods = new List<Type>() { typeof(VaderInDefender) };
                ImageUrl = "https://i.imgur.com/1YLmbYQ.png";
            }
        }
    }
}
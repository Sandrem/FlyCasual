using Mods.ModsList;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.RZ1AWing
    {
        public class SharaBey : RZ1AWing
        {
            public SharaBey() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Shara Bey",
                    4,
                    34,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.SharaBeyAbility),
                    extraUpgradeIcons: new List<UpgradeType>() { UpgradeType.Talent, UpgradeType.Talent },
                    abilityText: "While you defend or perform a primary attack, you may spend 1 lock you have on the enemy ship to add 1 focus result to your dice results."
                );

                RequiredMods = new List<Type>() { typeof(UnreleasedContentMod) };
                PilotNameCanonical = "sharabey-rz1awing";

                ModelInfo.SkinName = "Phoenix Squadron";
            }
        }
    }
}
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
                    32,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.SharaBeyAbility),
                    extraUpgradeIcons: new List<UpgradeType>() { UpgradeType.Talent, UpgradeType.Talent },
                    abilityText: "While you defend or perform a primary attack, you may spend 1 lock you have on the enemy ship to add 1 focus result to your dice results."
                );

                PilotNameCanonical = "sharabey-rz1awing";

                ModelInfo.SkinName = "Red";

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/7e/9a/7e9a07c1-934f-4229-b70f-c54f1c0a60de/swz83_pilot_sharabey.png";
            }
        }
    }
}
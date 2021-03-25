using System;
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
                PilotInfo = new PilotCardInfo(
                    "Vult Skerris",
                    5,
                    46,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.VultSkerrisDefenderAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    abilityText: " Action: Recover 1 charge take 1 strain. Before you engage you spend 1 charge to perform an action.",
                    charges: 1,
                    regensCharges: -1
                );

                PilotNameCanonical = "vultskerris-tieinterceptor";

                ModelInfo.SkinName = "Skystrike Academy";

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/fc/c9/fcc90b4b-afb5-4e62-a385-7053fde0d825/swz84_pilot_vultskerris.png";
            }
        }
    }
}
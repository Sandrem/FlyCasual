using Content;
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
                PilotInfo = new PilotCardInfo25
                (
                    "Vult Skerris",
                    "Arrogant Ace",
                    Faction.Imperial,
                    5,
                    4,
                    6,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.VultSkerrisDefenderAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    abilityText: " Action: Recover 1 charge take 1 strain. Before you engage you spend 1 charge to perform an action.",
                    charges: 1,
                    regensCharges: -1,
                    skinName: "Skystrike Academy"
                );

                PilotNameCanonical = "vultskerris-tieinterceptor";

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/fc/c9/fcc90b4b-afb5-4e62-a385-7053fde0d825/swz84_pilot_vultskerris.png";
            }
        }
    }
}
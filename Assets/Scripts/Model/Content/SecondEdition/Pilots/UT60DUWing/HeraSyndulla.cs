using Mods.ModsList;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.UT60DUWing
    {
        public class HeraSyndulla : UT60DUWing
        {
            public HeraSyndulla() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Hera Syndulla",
                    5,
                    51,
                    pilotTitle: "Spectre-2",
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.HeraSyndullaAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );

                RequiredMods = new List<Type>() { typeof(PhoenixSquadronModSE) };
                PilotNameCanonical = "herasyndulla-ut60duwing-phoenixsquadronmod";

                ImageUrl = "https://i.imgur.com/GdRl4Zt.png";
            }
        }
    }
}

using Abilities.SecondEdition;
using Mods.ModsList;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.T65XWing
    {
        public class HeraSyndulla : T65XWing
        {
            public HeraSyndulla() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Hera Syndulla",
                    5,
                    49,
                    pilotTitle: "Spectre-2",
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.HeraSyndullaAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );

                RequiredMods = new List<Type>() { typeof(PhoenixSquadronModSE) };
                PilotNameCanonical = "herasyndulla-t65xwing-phoenixsquadronmod";

                ImageUrl = "https://i.imgur.com/9m8Daqh.png";

                ModelInfo.SkinName = "Green";
            }
        }
    }
}

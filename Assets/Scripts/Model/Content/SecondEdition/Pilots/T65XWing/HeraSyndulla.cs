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

                RequiredMods = new List<Type>() { typeof(MyOtherRideIsSEMod) };
                ImageUrl = "https://i.imgur.com/BeI0SMc.png";
                ModelInfo.SkinName = "Green";
            }
        }
    }
}

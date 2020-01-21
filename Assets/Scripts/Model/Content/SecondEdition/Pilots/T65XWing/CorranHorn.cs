using Abilities.SecondEdition;
using Mods.ModsList;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.T65XWing
    {
        public class CorranHorn : T65XWing
        {
            public CorranHorn() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Corran Horn",
                    5,
                    55,
                    pilotTitle: "Tenacious Investigator",
                    isLimited: true,
                    abilityType: typeof(CorranHornAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );

                RequiredMods = new List<Type>() { typeof(MyOtherRideIsModSE) };
                PilotNameCanonical = "corranhorn-t65xwing-myotherrideismod";

                ImageUrl = "https://i.imgur.com/vGutvzp.png";

                ModelInfo.SkinName = "Green";
            }
        }
    }
}

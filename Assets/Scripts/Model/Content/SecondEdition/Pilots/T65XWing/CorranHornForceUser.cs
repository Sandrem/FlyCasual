using Abilities.SecondEdition;
using Mods.ModsList;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.T65XWing
    {
        public class CorranHornForceUser : T65XWing
        {
            public CorranHornForceUser() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Corran Horn (Force user)",
                    5,
                    63,
                    pilotTitle: "Tenacious Investigator",
                    isLimited: true,
                    force: 1,
                    abilityType: typeof(CorranHornAbility),
                    extraUpgradeIcon: UpgradeType.ForcePower
                );

                RequiredMods = new List<Type>() { typeof(MyOtherRideIsModSE) };
                PilotNameCanonical = "corranhornforceuser-t65xwing-myotherrideismod";

                ImageUrl = "https://i.imgur.com/Zaw8eDC.png";

                ModelInfo.SkinName = "Green";
            }
        }
    }
}

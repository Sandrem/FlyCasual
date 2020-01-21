using Abilities.SecondEdition;
using Mods.ModsList;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.T65XWing
    {
        public class TychoCelchu : T65XWing
        {
            public TychoCelchu() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Tycho Celchu",
                    5,
                    50,
                    pilotTitle: "Master of Stress",
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.TychoCelchuAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );

                RequiredMods = new List<Type>() { typeof(MyOtherRideIsModSE) };
                PilotNameCanonical = "tychocelchu-t65xwing-myotherrideismod";

                ImageUrl = "https://i.imgur.com/wGvWi6p.png";
            }
        }
    }
}

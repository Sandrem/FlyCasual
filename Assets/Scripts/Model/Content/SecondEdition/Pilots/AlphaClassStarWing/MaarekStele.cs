using Mods.ModsList;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.AlphaClassStarWing
    {
        public class MaarekStele : AlphaClassStarWing
        {
            public MaarekStele() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Maarek Stele",
                    5,
                    42,
                    pilotTitle: "Servant of the Empire",
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.MaarekSteleAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );

                RequiredMods = new List<Type>() { typeof(MyOtherRideIsModSE) };
                PilotNameCanonical = "maarekstele-alphaclassstarwing-myotherrideismod";

                ImageUrl = "https://i.imgur.com/pDPWfAz.png";
            }
        }
    }
}

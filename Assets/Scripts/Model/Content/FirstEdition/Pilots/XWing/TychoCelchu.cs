using System.Collections;
using System.Collections.Generic;
using Mods.ModsList;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.XWing
    {
        public class TychoCelchu : XWing
        {
            public TychoCelchu() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Tycho Celchu",
                    8,
                    28,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.TychoCelchuAbility),
                    extraUpgradeIcon: UpgradeType.Elite
                );

                RequiredMods.Add(typeof(MyOtherRideIsMod));
                ImageUrl = "https://i.imgur.com/imayMBg.png";
            }
        }
    }
}
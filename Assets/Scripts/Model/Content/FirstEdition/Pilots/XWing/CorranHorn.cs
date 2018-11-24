using System.Collections;
using System.Collections.Generic;
using Mods.ModsList;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.XWing
    {
        public class CorranHorn : XWing
        {
            public CorranHorn() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Corran Horn",
                    8,
                    28,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.CorranHornAbility),
                    extraUpgradeIcon: UpgradeType.Elite
                );

                RequiredMods.Add(typeof(MyOtherRideIsMod));
                ImageUrl = "https://i.imgur.com/3uwdfCc.png";

                ModelInfo.SkinName = "Green";
            }
        }
    }
}
using Mods.ModsList;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.BWing
    {
        public class BraylenStramm : BWing
        {
            public BraylenStramm() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Braylen Stramm",
                    3,
                    24,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.BraylenStrammAbility),
                    extraUpgradeIcon: UpgradeType.Elite
                );

                RequiredMods.Add(typeof(MyOtherRideIsMod));
                ImageUrl = "https://i.imgur.com/V6m7JN9.png";

                ModelInfo.SkinName = "Dark Blue";
            }
        }
    }
}

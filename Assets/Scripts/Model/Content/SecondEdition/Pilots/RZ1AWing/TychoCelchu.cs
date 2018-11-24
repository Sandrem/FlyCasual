using Mods.ModsList;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.RZ1AWing
    {
        public class TychoCelchu : RZ1AWing
        {
            public TychoCelchu() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Tycho Celchu",
                    5,
                    42,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.TychoCelchuAbility),
                    extraUpgradeIcon: UpgradeType.Elite
                );
                
                RequiredMods = new List<System.Type>() { typeof(FirstEditionPilotsMod) };
                ImageUrl = "https://i.imgur.com/lzgv9da.png";
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;

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
                    abilityType: typeof(Abilities.FirstEdition.TychoCelchuAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(Upgrade.UpgradeType.Elite);

                ImageUrl = "https://i.imgur.com/lzgv9da.png";

                RequiredMods = new List<System.Type>() { typeof(Mods.ModsList.FirstEditionPilotsMod) };
            }
        }
    }
}
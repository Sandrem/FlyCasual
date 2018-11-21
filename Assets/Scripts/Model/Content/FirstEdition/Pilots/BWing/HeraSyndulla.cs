using Mods.ModsList;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.BWing
    {
        public class HeraSyndulla : BWing
        {
            public HeraSyndulla() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Hera Syndulla",
                    7,
                    29,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.HeraSyndullaAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);

                ImageUrl = "https://i.imgur.com/L6wpW8S.png";

                RequiredMods.Add(typeof(PhoenixSquadronMod));
            }
        }
    }
}

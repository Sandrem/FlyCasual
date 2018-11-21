using System.Collections;
using System.Collections.Generic;
using Mods.ModsList;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.AWing
    {
        public class SabineWren : AWing
        {
            public SabineWren() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Sabine Wren",
                    5,
                    23,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.HeraSyndullaAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);

                RequiredMods.Add(typeof(PhoenixSquadronMod));

                ImageUrl = "https://i.imgur.com/yRrheRR.png";

                ModelInfo.SkinName = "Green";
            }
        }
    }
}
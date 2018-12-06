using System.Collections;
using System.Collections.Generic;
using Mods.ModsList;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.AWing
    {
        public class EzraBridger : AWing
        {
            public EzraBridger() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Ezra Bridger",
                    4,
                    22,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.EzraBridgerPilotAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );

                RequiredMods.Add(typeof(PhoenixSquadronMod));
                ImageUrl = "https://i.imgur.com/xPe8HQo.png";

                ModelInfo.SkinName = "Blue";
            }
        }
    }
}
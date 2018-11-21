using Mods.ModsList;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.AlphaClassStarWing
    {
        public class MaarekStele : AlphaClassStarWing
        {
            public MaarekStele() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Maarek Stele",
                    7,
                    27,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.MaarekSteleAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);

                ModelInfo.SkinName = "Red";

                RequiredMods.Add(typeof(MyOtherRideIsMod));

                ImageUrl = "https://i.imgur.com/SFGZXbc.png";
            }
        }
    }
}

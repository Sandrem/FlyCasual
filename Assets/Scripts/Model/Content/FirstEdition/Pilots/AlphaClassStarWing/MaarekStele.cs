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
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.MaarekSteleAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );

                ModelInfo.SkinName = "Red";

                IsHidden = true;

                ImageUrl = "https://i.imgur.com/SFGZXbc.png";
            }
        }
    }
}

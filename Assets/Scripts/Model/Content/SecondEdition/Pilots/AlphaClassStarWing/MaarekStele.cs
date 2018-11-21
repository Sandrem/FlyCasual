using Upgrade;

namespace Ship
{
    namespace SecondEdition.AlphaClassStarWing
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
                    abilityType: typeof(Abilities.FirstEdition.LieutenantKarsabiAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);

                SEImageNumber = 136;
            }
        }
    }
}

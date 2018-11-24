using Upgrade;

namespace Ship
{
    namespace SecondEdition.AlphaClassStarWing
    {
        public class LieutenantKarsabi : AlphaClassStarWing
        {
            public LieutenantKarsabi() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Lieutenant Karsabi",
                    3,
                    39,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.LieutenantKarsabiAbility),
                    extraUpgradeIcon: UpgradeType.Elite,
                    seImageNumber: 136
                );
            }
        }
    }
}

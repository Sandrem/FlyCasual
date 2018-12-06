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
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.LieutenantKarsabiAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 136
                );
            }
        }
    }
}

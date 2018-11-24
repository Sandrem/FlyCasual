using Upgrade;

namespace Ship
{
    namespace SecondEdition.StarViperClassAttackPlatform
    {
        public class BlackSunAssassin : StarViperClassAttackPlatform
        {
            public BlackSunAssassin() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Black Sun Enforcer",
                    3,
                    48,
                    extraUpgradeIcon: UpgradeType.Elite,
                    seImageNumber: 181
                );

                ModelInfo.SkinName = "Black Sun Assassin";
            }
        }
    }
}
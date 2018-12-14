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
                    "Black Sun Assassin",
                    3,
                    48,
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 181
                );

                ModelInfo.SkinName = "Black Sun Assassin";
            }
        }
    }
}
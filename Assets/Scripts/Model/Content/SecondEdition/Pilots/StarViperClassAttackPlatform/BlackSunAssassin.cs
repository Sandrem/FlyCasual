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
                    48
                );

                ModelInfo.SkinName = "Black Sun Assassin";

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);

                SEImageNumber = 181;
            }
        }
    }
}
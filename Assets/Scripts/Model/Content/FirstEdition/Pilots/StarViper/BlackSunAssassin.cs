using Upgrade;

namespace Ship
{
    namespace FirstEdition.StarViper
    {
        public class BlackSunAssassin : StarViper
        {
            public BlackSunAssassin() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Black Sun Assassin",
                    5,
                    28,
                    extraUpgradeIcon: UpgradeType.Talent
                );

                ModelInfo.SkinName = "Black Sun Assassin";
            }
        }
    }
}

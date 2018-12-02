using Upgrade;

namespace Ship
{
    namespace FirstEdition.G1AStarfighter
    {
        public class GandFindsman : G1AStarfighter
        {
            public GandFindsman() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Gand Findsman",
                    5,
                    25,
                    extraUpgradeIcon: UpgradeType.Talent
                );
            }
        }
    }
}

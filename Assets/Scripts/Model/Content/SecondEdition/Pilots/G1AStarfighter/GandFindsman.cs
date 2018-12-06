using Upgrade;

namespace Ship
{
    namespace SecondEdition.G1AStarfighter
    {
        public class GandFindsman : G1AStarfighter
        {
            public GandFindsman() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Gand Findsman",
                    1,
                    43,
                    seImageNumber: 203
                );
            }
        }
    }
}
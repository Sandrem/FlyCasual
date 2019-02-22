using Upgrade;

namespace Ship
{
    namespace SecondEdition.ScurrgH6Bomber
    {
        public class LokRevenant : ScurrgH6Bomber
        {
            public LokRevenant() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Lok Revenant",
                    2,
                    43,
                    seImageNumber: 206
                );
            }
        }
    }
}
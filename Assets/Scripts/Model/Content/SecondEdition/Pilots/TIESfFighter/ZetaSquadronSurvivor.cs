using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIESfFighter
    {
        public class ZetaSquadronSurvivor : TIESfFighter
        {
            public ZetaSquadronSurvivor() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Zeta Squadron Survivor",
                    1,
                    1 //,
                      //seImageNumber: 120
                );              
            }
        }
    }
}

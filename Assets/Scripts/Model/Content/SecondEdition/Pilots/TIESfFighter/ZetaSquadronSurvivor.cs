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
                    2,
                    34 //,
                      //seImageNumber: 120
                );

                ImageUrl = "http://www.infinitearenas.com/xw2browse/images/first-order/zeta-squadron-survivor.jpg";
            }
        }
    }
}

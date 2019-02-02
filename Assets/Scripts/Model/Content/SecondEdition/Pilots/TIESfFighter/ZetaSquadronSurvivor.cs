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

                ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/848db1993150bda19217e2c14b3c3df6.png";
            }
        }
    }
}

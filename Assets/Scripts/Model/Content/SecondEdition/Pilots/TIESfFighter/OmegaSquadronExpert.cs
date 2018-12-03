using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIESfFighter
    {
        public class OmegaSquadronExpert : TIESfFighter
        {
            public OmegaSquadronExpert() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Omega Squadron Expert",
                    3,
                    36,
                    extraUpgradeIcon: UpgradeType.Talent //,
                      //seImageNumber: 120
                );

                ImageUrl = "http://www.infinitearenas.com/xw2browse/images/first-order/omega-squadron-expert.jpg";
            }
        }
    }
}

using Upgrade;

namespace Ship
{
    namespace SecondEdition.UpsilonClassCommandShuttle
    {
        public class StarkillerBasePilot : UpsilonClassCommandShuttle
        {
            public StarkillerBasePilot() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Starkiller Base Pilot",
                    2,
                    56 //,
                    //seImageNumber: 120
                );

                ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/41f6d936f14a058ed1c5e6ac12de37c2.png";
            }
        }
    }
}

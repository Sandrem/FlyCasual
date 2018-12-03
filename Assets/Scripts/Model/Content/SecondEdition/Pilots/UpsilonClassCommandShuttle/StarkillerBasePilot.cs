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

                ImageUrl = "http://www.infinitearenas.com/xw2browse/images/first-order/starkiller-base-pilot.jpg";
            }
        }
    }
}

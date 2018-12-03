using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEVnSilencer
    {
        public class FirstOrderTestPilot : TIEVnSilencer
        {
            public FirstOrderTestPilot() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "First Order Test Pilot",
                    4,
                    62,
                    extraUpgradeIcon: UpgradeType.Talent //,
                    //seImageNumber: 120
                );

                ImageUrl = "http://www.infinitearenas.com/xw2browse/images/first-order/first-order-test-pilot.jpg";
            }
        }
    }
}

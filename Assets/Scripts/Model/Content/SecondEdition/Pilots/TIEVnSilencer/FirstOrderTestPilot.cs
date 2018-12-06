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
                    1,
                    1 //,
                    //seImageNumber: 120
                );
            }
        }
    }
}

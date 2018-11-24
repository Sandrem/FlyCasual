using Upgrade;

namespace Ship
{
    namespace FirstEdition.TIESilencer
    {
        public class FirstOrderTestPilot : TIESilencer
        {
            public FirstOrderTestPilot() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "First Order Test Pilot",
                    6,
                    29,
                    extraUpgradeIcon: UpgradeType.Elite
                );
            }
        }
    }
}
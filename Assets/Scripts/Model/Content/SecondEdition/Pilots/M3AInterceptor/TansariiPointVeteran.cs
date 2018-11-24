using Upgrade;

namespace Ship
{
    namespace SecondEdition.M3AInterceptor
    {
        public class TansariiPointVeteran : M3AInterceptor
        {
            public TansariiPointVeteran() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Tansarii Point Veteran",
                    3,
                    33,
                    extraUpgradeIcon: UpgradeType.Elite,
                    seImageNumber: 189
                );
            }
        }
    }
}
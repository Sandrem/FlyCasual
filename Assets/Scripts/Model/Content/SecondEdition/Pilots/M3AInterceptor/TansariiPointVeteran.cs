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
                    33
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);

                SEImageNumber = 189;
            }
        }
    }
}
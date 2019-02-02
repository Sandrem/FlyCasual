using Upgrade;

namespace Ship
{
    namespace FirstEdition.M3AInterceptor
    {
        public class TansariiPointVeteran : M3AInterceptor
        {
            public TansariiPointVeteran() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Tansarii Point Veteran",
                    5,
                    17,
                    extraUpgradeIcon: UpgradeType.Talent
                );

                ModelInfo.SkinName = "Serissu";
            }
        }
    }
}

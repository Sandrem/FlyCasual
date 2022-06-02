using Upgrade;

namespace Ship
{
    namespace SecondEdition.M3AInterceptor
    {
        public class TansariiPointVeteran : M3AInterceptor
        {
            public TansariiPointVeteran() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Tansarii Point Veteran",
                    "",
                    Faction.Scum,
                    3,
                    3,
                    3,
                    seImageNumber: 189
                );
            }
        }
    }
}
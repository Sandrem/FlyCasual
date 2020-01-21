using System;
using System.Linq;
using BoardTools;
using Ship;
using SubPhases;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEBaInterceptor
    {
        public class FirstOrderProvocateur : TIEBaInterceptor
        {
            public FirstOrderProvocateur() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "First Order Provocateur",
                    3,
                    45
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/ef/d7/efd7cc94-bdf7-4f63-80eb-476444dfeb28/swz62_card_first-order-provocateur.png";
            }
        }
    }
}

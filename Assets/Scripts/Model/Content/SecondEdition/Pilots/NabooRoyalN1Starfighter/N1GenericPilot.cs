using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.NabooRoyalN1Starfighter
    {
        public class N1GenericPilot : NabooRoyalN1Starfighter
        {
            public N1GenericPilot() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "N-1 Generic Pilot",
                    2,
                    30
                );

                ImageUrl = "https://i.imgur.com/nPZzZUW.png";
            }
        }
    }
}

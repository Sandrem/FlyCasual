using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.NabooRoyalN1Starfighter
    {
        public class BravoFlightOfficer : NabooRoyalN1Starfighter
        {
            public BravoFlightOfficer() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Bravo Flight Officer",
                    2,
                    33
                );

                ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/274db1f70ed4da939b9440837a30c39a.png";
            }
        }
    }
}

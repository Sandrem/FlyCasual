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
                    30
                );

                ImageUrl = "https://i.imgur.com/nPZzZUW.png";
            }
        }
    }
}

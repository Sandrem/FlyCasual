using System;
using System.Collections.Generic;

namespace Ship
{
    namespace SecondEdition.NimbusClassVWing
    {
        public class LoyalistVolunteer : NimbusClassVWing
        {
            public LoyalistVolunteer() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Loyalist Volunteer",
                    2,
                    30
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/a1/27/a127938a-aecc-48eb-ba09-622781e84084/swz80_ship_volunteer.png";
            }
        }
    }
}
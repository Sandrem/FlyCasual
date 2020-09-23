using System.Collections;
using System.Collections.Generic;

namespace Ship
{
    namespace SecondEdition.LaatIGunship
    {
        public class P212thBattalionPilot : LaatIGunship
        {
            public P212thBattalionPilot() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "212th Battalion Pilot",
                    2,
                    51
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/80/e7/80e7698b-13df-4d66-ba2b-575df467a7df/swz70_a1_battalion-pilot_ship.png";
            }
        }
    }
}
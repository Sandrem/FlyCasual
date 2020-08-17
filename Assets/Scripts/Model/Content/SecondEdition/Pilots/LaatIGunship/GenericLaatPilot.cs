using System.Collections;
using System.Collections.Generic;

namespace Ship
{
    namespace SecondEdition.LaatIGunship
    {
        public class GenericLaatPilot : LaatIGunship
        {
            public GenericLaatPilot() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Generic Laat Pilot",
                    2,
                    40
                );
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;

namespace Ship
{
    namespace SecondEdition.UT60DUWing
    {
        public class BlueSquadronScout : UT60DUWing
        {
            public BlueSquadronScout() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Blue Squadron Scout",
                    2,
                    43,
                    seImageNumber: 60
                );
            }
        }
    }
}

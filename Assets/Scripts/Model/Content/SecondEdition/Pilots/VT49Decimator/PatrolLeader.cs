using System.Collections;
using System.Collections.Generic;

namespace Ship
{
    namespace SecondEdition.VT49Decimator
    {
        public class PatrolLeader : VT49Decimator
        {
            public PatrolLeader() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Patrol Leader",
                    2,
                    80
                );

                SEImageNumber = 148;
            }
        }
    }
}

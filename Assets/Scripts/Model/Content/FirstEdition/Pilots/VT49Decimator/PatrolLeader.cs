using System.Collections;
using System.Collections.Generic;

namespace Ship
{
    namespace FirstEdition.VT49Decimator
    {
        public class PatrolLeader : VT49Decimator
        {
            public PatrolLeader() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Patrol Leader",
                    3,
                    40
                );
            }
        }
    }
}

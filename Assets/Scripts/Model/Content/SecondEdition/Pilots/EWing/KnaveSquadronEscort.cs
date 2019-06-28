using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace SecondEdition.EWing
    {
        public class KnaveSquadronEscort : EWing
        {
            public KnaveSquadronEscort() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Knave Squadron Escort",
                    2,
                    52,
                    seImageNumber: 53
                );
            }
        }
    }
}

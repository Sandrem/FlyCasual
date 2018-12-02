using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace FirstEdition.EWing
    {
        public class KnaveSquadronPilot : EWing
        {
            public KnaveSquadronPilot() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Knave Squadron Pilot",
                    1,
                    27
                );
            }
        }
    }
}

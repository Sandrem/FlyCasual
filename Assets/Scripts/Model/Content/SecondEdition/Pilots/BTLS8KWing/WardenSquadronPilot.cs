using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.BTLS8KWing
    {
        public class WardenSquadronPilot : BTLS8KWing
        {
            public WardenSquadronPilot() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Warden Squadron Pilot",
                    2,
                    37,
                    seImageNumber: 64
                );
            }
        }
    }
}

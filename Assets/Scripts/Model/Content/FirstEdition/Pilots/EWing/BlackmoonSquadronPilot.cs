using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace FirstEdition.EWing
    {
        public class BlackmoonSquadronPilot : EWing
        {
            public BlackmoonSquadronPilot() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Blackmoon Squadron Pilot",
                    3,
                    29
                );
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace FirstEdition.XWing
    {
        public class RedSquadronPilot : XWing
        {
            public RedSquadronPilot() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Red Squadron Pilot",
                    4,
                    23
                );
            }
        }
    }
}

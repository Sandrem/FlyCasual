using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace FirstEdition.YWing
    {
        public class GraySquadronPilot : YWing
        {
            public GraySquadronPilot() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Gray Squadron Pilot",
                    4,
                    20
                );
            }
        }
    }
}

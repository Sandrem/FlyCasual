using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace FirstEdition.YWing
    {
        public class GoldSquadronPilot : YWing
        {
            public GoldSquadronPilot() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Gold Squadron Pilot",
                    2,
                    18
                );
            }
        }
    }
}

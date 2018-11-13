using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace FirstEdition.XWing
    {
        public class RookiePilot : XWing
        {
            public RookiePilot() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Rookie Pilot",
                    2,
                    21
                );
            }
        }
    }
}

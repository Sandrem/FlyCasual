using Ship;
using System;
using System.Collections.Generic;

namespace Ship
{
    namespace SecondEdition.TIEInterceptor
    {
        public class AlphaSquadronPilot : TIEInterceptor
        {
            public AlphaSquadronPilot() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Alpha Squadron Pilot",
                    1,
                    34,
                    seImageNumber: 106
                );
            }
        }
    }
}
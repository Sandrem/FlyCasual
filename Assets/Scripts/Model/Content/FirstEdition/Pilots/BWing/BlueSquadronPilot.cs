using System.Collections;
using System.Collections.Generic;

namespace Ship
{
    namespace FirstEdition.BWing
    {
        public class BlueSquadronPilot : BWing
        {
            public BlueSquadronPilot() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Blue Squadron Pilot",
                    2,
                    22
                );
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.ASF01BWing
    {
        public class BlueSquadronPilot : ASF01BWing
        {
            public BlueSquadronPilot() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Blue Squadron Pilot",
                    2,
                    42
                );

                SEImageNumber = 26;
            }
        }
    }
}

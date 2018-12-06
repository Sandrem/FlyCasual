using System.Collections;
using System.Collections.Generic;

namespace Ship
{
    namespace FirstEdition.KWing
    {
        public class GuardianSquadronPilot : KWing
        {
            public GuardianSquadronPilot() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Guardian Squadron Pilot",
                    4,
                    25
                );
            }
        }
    }
}

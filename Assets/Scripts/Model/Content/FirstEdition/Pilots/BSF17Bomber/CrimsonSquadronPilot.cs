using System.Collections;
using System.Collections.Generic;

namespace Ship
{
    namespace FirstEdition.BSF17Bomber
    {
        public class CrimsonSquadronPilot : BSF17Bomber
        {
            public CrimsonSquadronPilot() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Crimson Squadron Pilot",
                    1,
                    25
                );
            }
        }
    }
}

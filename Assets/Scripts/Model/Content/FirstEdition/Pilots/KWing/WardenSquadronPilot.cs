using System.Collections;
using System.Collections.Generic;

namespace Ship
{
    namespace FirstEdition.KWing
    {
        public class WardenSquadronPilot : KWing
        {
            public WardenSquadronPilot() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Warden Squadron Pilot",
                    2,
                    23
                );
            }
        }
    }
}

using System;
using System.Linq;
using BoardTools;
using Ship;
using SubPhases;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.Fireball
    {
        public class ColossusStationMechanic : Fireball
        {
            public ColossusStationMechanic() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Colossus Station Mechanic",
                    2,
                    26
                );
            }
        }
    }
}
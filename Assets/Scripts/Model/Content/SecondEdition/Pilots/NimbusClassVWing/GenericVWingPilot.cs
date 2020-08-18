using System;
using System.Collections.Generic;
using Ship;
using SubPhases;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.NimbusClassVWing
    {
        public class GenericVWingPilot : NimbusClassVWing
        {
            public GenericVWingPilot() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Generic V-wing Pilot",
                    2,
                    30
                );
            }
        }
    }
}
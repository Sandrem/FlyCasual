using System.Collections.Generic;

namespace Ship
{
    namespace SecondEdition.TIEAdvancedX1
    {
        public class TempestSquadronPilot : TIEAdvancedX1
        {
            public TempestSquadronPilot() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Tempest Squadron Pilot",
                    2,
                    41,
                    seImageNumber: 98
                );
            }
        }
    }
}
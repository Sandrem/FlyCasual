using BoardTools;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ship.SecondEdition.ResistanceTransport
{
    public class LogisticsDivisionPilot : ResistanceTransport
    {
        public LogisticsDivisionPilot()
        {
            PilotInfo = new PilotCardInfo(
                "Logistics Division Pilot",
                1,
                32
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/95/69/95692636-a7ef-499c-afb7-5891b998f696/swz45_logistics-pilot.png";
        }
    }
}
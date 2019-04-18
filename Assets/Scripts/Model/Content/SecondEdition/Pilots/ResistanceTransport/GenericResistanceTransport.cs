using BoardTools;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.ResistanceTransport
{
    public class GenericResistanceTransport : ResistanceTransport
    {
        public GenericResistanceTransport()
        {
            PilotInfo = new PilotCardInfo(
                "Generic Resistance Transport",
                1,
                40
            );
        }
    }
}
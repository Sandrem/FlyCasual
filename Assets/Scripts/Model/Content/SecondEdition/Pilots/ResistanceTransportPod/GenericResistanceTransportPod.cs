using BoardTools;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.ResistanceTransportPod
{
    public class GenericResistanceTransportPod : ResistanceTransportPod
    {
        public GenericResistanceTransportPod()
        {
            PilotInfo = new PilotCardInfo(
                "Generic Resistance Transport Pod",
                1,
                30
            );
        }
    }
}
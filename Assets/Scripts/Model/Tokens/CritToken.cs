using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tokens
{

    public class CritToken : GenericToken
    {
        public CritToken() {
            Name = "Critical Hit Token";
            Temporary = false;
        }
    }

    public class BlindedPilotCritToken : CritToken
    {
        public BlindedPilotCritToken() : base()
        {
            Tooltip = "https://images-cdn.fantasyflightgames.com/filer_public/52/0d/520d9611-3a61-473d-8ebe-19c5659a9c6b/blinded-pilot.png";
        }
    }

    public class DamagedCockpitCritToken : CritToken
    {
        public DamagedCockpitCritToken() : base()
        {
            Tooltip = "http://i.imgur.com/bNfmyKe.png";
        }
    }
}

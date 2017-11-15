using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEDefender
    {
        public class DeltaSquadronPilot : TIEDefender
        {
            public DeltaSquadronPilot() : base()
            {
                PilotName = "Delta Squadron Pilot";
                PilotSkill = 1;
                Cost = 30;
            }
        }
    }
}

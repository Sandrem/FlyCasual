using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEInterceptor
    {
        public class AlphaSquadronPilot : TIEInterceptor
        {
            public AlphaSquadronPilot() : base()
            {
                PilotName = "Alpha Squadron Pilot";
                PilotSkill = 1;
                Cost = 18;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEInterceptor
    {
        public class AvengerSquadronPilot : TIEInterceptor
        {
            public AvengerSquadronPilot() : base()
            {
                PilotName = "Avenger Squadron Pilot";
                PilotSkill = 3;
                Cost = 20;
            }
        }
    }
}

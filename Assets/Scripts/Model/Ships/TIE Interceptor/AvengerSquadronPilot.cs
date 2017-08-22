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
                ImageUrl = "https://vignette3.wikia.nocookie.net/xwing-miniatures/images/5/50/Avenger_Squadron_Pilot.png";
                PilotSkill = 3;
                Cost = 20;
            }
        }
    }
}

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
                ImageUrl = "https://vignette4.wikia.nocookie.net/xwing-miniatures/images/c/c2/Alpha_Squadron_Pilot.png";
                PilotSkill = 1;
                Cost = 18;
            }
        }
    }
}

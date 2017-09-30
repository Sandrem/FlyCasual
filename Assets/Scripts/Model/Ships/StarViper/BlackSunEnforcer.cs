using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace StarViper
    {
        public class BlackSunEnforcer : StarViper
        {
            public BlackSunEnforcer() : base()
            {
                PilotName = "Black Sun Enforcer";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Scum%20and%20Villainy/StarViper/black-sun-enforcer.png";
                PilotSkill = 1;
                Cost = 25;
            }
        }
    }
}

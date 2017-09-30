using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEAdvPrototype
    {
        public class SienarTestPilot : TIEAdvPrototype
        {
            public SienarTestPilot() : base()
            {
                PilotName = "Sienar Test Pilot";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Galactic%20Empire/TIE%20Adv.%20Prototype/sienar-test-pilot.png";
                PilotSkill = 2;
                Cost = 16;
            }
        }
    }
}

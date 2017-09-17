using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEAgressor
    {
        public class SienarSpecialist : TIEAgressor
        {
            public SienarSpecialist() : base()
            {
                PilotName = "Sienar Specialist";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Galactic%20Empire/TIE%20Aggressor/sienar-specialist.png";
                PilotSkill = 2;
                Cost = 17;
            }
        }
    }
}

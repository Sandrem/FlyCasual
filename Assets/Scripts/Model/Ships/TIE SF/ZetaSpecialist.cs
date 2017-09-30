using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIESF
    {
        public class ZetaSpecialist : TIESF
        {
            public ZetaSpecialist() : base()
            {
                PilotName = "Zeta Specialist";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/First%20Order/TIE-sf%20Fighter/zeta-specialist.png";
                PilotSkill = 3;
                Cost = 23;
            }
        }
    }
}

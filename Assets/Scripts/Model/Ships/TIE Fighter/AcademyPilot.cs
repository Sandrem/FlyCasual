using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEFighter
    {
        public class AcademyPilot: TIEFighter
        {
            public AcademyPilot() : base()
            {
                PilotName = "Academy Pilot";
                ImageUrl = "https://vignette2.wikia.nocookie.net/xwing-miniatures/images/4/41/Academy-pilot.png";
                PilotSkill = 1;
            }
        }
    }
}

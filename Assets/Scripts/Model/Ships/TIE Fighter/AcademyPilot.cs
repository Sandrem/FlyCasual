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
                PilotSkill = 1;
                Cost = 12;
            }
        }
    }
}

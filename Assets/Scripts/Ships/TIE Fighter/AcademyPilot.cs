using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEFighter
    {
        public class AcademyPilot: TIEFighter
        {
            public AcademyPilot(Players.GenericPlayer owner, int shipId, Vector3 position) : base(owner, shipId, position)
            {
                PilotName = "Academy Pilot";
                PilotSkill = 1;
            }
        }
    }
}

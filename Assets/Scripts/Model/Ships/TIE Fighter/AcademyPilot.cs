using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEFighter
    {
        public class AcademyPilot: TIEFighter
        {
            public AcademyPilot(Players.PlayerNo playerNo, int shipId, Vector3 position) : base(playerNo, shipId, position)
            {
                PilotName = "Academy Pilot";
                PilotSkill = 1;

                InitializePilot();
            }
        }
    }
}

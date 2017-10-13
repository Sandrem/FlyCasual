using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEFO
    {
        public class EpsilonSquadronPilot : TIEFO
        {
            public EpsilonSquadronPilot() : base()
            {
                PilotName = "Epsilon Squadron Pilot";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/First%20Order/TIE-fo%20Fighter/epsilon-squadron-pilot.png";
                PilotSkill = 1;
                Cost = 15;
            }
        }
    }
}

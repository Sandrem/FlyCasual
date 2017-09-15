using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace KWing
    {
        public class GuardianSquadronPilot : KWing
        {
            public GuardianSquadronPilot() : base()
            {
                PilotName = "Warden Squadron Pilot";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Rebel%20Alliance/K-wing/guardian-squadron-pilot.png";
                PilotSkill = 4;
                Cost = 25;
            }
        }
    }
}

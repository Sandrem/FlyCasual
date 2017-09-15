using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace KWing
    {
        public class WardenSquadronPilot : KWing
        {
            public WardenSquadronPilot() : base()
            {
                PilotName = "Warden Squadron Pilot";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Rebel%20Alliance/K-wing/warden-squadron-pilot.png";
                PilotSkill = 2;
                Cost = 23;
            }
        }
    }
}

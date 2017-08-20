using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace Z95
    {
        public class BanditSquadronPilot : Z95
        {
            public BanditSquadronPilot() : base()
            {
                PilotName = "Bandit Squadron Pilot";
                ImageUrl = "https://vignette3.wikia.nocookie.net/xwing-miniatures/images/5/5d/Bandit-squadron-pilot.png";
                PilotSkill = 1;
                Cost = 12;
            }
        }
    }
}

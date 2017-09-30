using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace UpsilonShuttle
    {
        public class StarkillerBasePilot : UpsilonShuttle
        {
            public StarkillerBasePilot() : base()
            {
                PilotName = "Starkiller Base Pilot";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/First%20Order/Upsilon-class%20Shuttle/starkiller-base-pilot.png";
                PilotSkill = 2;
                Cost = 30;
            }
        }
    }
}

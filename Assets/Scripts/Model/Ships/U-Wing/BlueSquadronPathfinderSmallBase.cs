using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace UWing
    {
        public class BlueSquadronPathfinderSmallBase : UWingSmallBase
        {
            public BlueSquadronPathfinderSmallBase() : base()
            {
                PilotName = "Blue Squadron Pathfinder (Small Base)";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Rebel%20Alliance/U-wing/blue-squadron-pathfinder.png";
                PilotSkill = 2;
                Cost = 23;

                IsCustomContent = true;
            }
        }
    }
}

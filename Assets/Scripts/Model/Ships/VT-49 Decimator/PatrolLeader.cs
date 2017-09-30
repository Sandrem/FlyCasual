using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace VT49Decimator
    {
        public class PatrolLeader : VT49Decimator
        {
            public PatrolLeader() : base()
            {
                PilotName = "Patrol Leader";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Galactic%20Empire/VT-49%20Decimator/patrol-leader.png";
                PilotSkill = 3;
                Cost = 40;
            }
        }
    }
}

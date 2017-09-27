using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace G1AStarfighter
    {
        public class RuthlessFreelancer : G1AStarfighter
        {
            public RuthlessFreelancer() : base()
            {
                PilotName = "Ruthless Freelancer";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Scum%20and%20Villainy/G-1A%20Starfighter/ruthless-freelancer.png";
                PilotSkill = 3;
                Cost = 23;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace ProtectorateStarfighter
    {
        public class ZealousRecruit : ProtectorateStarfighter
        {
            public ZealousRecruit() : base()
            {
                PilotName = "Zealous Recruit";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Scum%20and%20Villainy/Protectorate%20Starfighter/zealous-recruit.png";
                PilotSkill = 1;
                Cost = 20;
            }
        }
    }
}

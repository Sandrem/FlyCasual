using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace ARC170
    {
        public class BraylenStramm : ARC170
        {
            public BraylenStramm() : base()
            {
                PilotName = "ARC Generic";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Rebel%20Alliance/ARC-170/braylen-stramm.png";
                PilotSkill = 3;
                Cost = 25;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace YT2400
    {
        public class WildSpaceFringer : YT2400
        {
            public WildSpaceFringer() : base()
            {
                PilotName = "Wild Space Fringer";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Rebel%20Alliance/YT-2400/wild-space-fringer.png";
                PilotSkill = 2;
                Cost = 30;
            }
        }
    }
}

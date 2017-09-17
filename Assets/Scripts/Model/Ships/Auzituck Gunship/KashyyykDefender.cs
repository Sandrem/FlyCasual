using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace AuzituckGunship
    {
        public class KashyyykDefender : AuzituckGunship
        {
            public KashyyykDefender() : base()
            {
                PilotName = "Kashyyyk Defender";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Rebel%20Alliance/Auzituck%20Gunship/kashyyyk-defender.png";
                PilotSkill = 1;
                Cost = 24;
            }
        }
    }
}

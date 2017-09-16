using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace T70XWing
    {
        public class BlueSquadronNovice : T70XWing
        {
            public BlueSquadronNovice() : base()
            {
                PilotName = "Blue Squadron Novice";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Resistance/T-70%20X-wing/blue-squadron-novice.png";
                PilotSkill = 2;
                Cost = 24;
            }
        }
    }
}

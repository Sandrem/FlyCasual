using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace YWing
    {
        public class GoldSquadronPilot : YWing
        {
            public GoldSquadronPilot() : base()
            {
                PilotName = "Gold Squadron Pilot";
                ImageUrl = "https://vignette2.wikia.nocookie.net/xwing-miniatures/images/d/d7/Gold_Squadron_Pilot.jpg";
                PilotSkill = 2;
                Cost = 18;
            }
        }
    }
}

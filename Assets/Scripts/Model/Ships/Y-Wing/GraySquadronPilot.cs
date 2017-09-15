using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace YWing
    {
        public class GraySquadronPilot : YWing
        {
            public GraySquadronPilot() : base()
            {
                PilotName = "Gray Squadron Pilot";
                ImageUrl = "https://vignette3.wikia.nocookie.net/xwing-miniatures/images/c/ca/Grey_Squadron_Pilot.jpg";
                PilotSkill = 4;
                Cost = 20;

                nameOfSkin = "Gray";
            }
        }
    }
}

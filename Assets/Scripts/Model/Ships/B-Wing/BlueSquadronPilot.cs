using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace BWing
    {
        public class BlueSquadronPilot : BWing
        {
            public BlueSquadronPilot() : base()
            {
                PilotName = "Blue Squadron Pilot";
                ImageUrl = "https://vignette4.wikia.nocookie.net/xwing-miniatures/images/6/61/Blue-squadron-pilot.png";
                PilotSkill = 2;
                Cost = 22;

                SkinName = "Blue";
            }
        }
    }
}

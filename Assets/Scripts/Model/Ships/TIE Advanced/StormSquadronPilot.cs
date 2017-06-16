using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEAdvanced
    {
        public class StormSquadronPilot : TIEAdvanced
        {
            public StormSquadronPilot() : base()
            {
                PilotName = "Storm Squadron Pilot";
                ImageUrl = "https://vignette3.wikia.nocookie.net/xwing-miniatures/images/6/67/Storm_Squadron_Pilot.jpg";
                PilotSkill = 4;
                Cost = 23;
            }
        }
    }
}

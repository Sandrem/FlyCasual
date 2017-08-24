using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEDefender
    {
        public class OnyxSquadronPilot : TIEDefender
        {
            public OnyxSquadronPilot() : base()
            {
                PilotName = "Onyx Squadron Pilot";
                ImageUrl = "https://vignette2.wikia.nocookie.net/xwing-miniatures/images/f/f0/Onyx_Squadron_Pilot.jpg";
                PilotSkill = 3;
                Cost = 32;
            }
        }
    }
}

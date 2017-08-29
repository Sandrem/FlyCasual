using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEDefender
    {
        public class DeltaSquadronPilot : TIEDefender
        {
            public DeltaSquadronPilot() : base()
            {
                PilotName = "Delta Squadron Pilot";
                ImageUrl = "https://vignette1.wikia.nocookie.net/xwing-miniatures/images/8/81/Delta-squadron-pilot.png";
                PilotSkill = 1;
                Cost = 30;
            }
        }
    }
}

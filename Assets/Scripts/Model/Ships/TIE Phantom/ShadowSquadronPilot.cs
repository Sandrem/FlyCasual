using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEPhantom
    {
        public class ShadowSquadronPilot : TIEPhantom
        {
            public ShadowSquadronPilot() : base()
            {
                PilotName = "Shadow Squadron Pilot";
                ImageUrl = "https://vignette2.wikia.nocookie.net/xwing-miniatures/images/0/0d/Shadow-squadron-pilot.png";
                PilotSkill = 5;
                Cost = 27;
            }
        }
    }
}

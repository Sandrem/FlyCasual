using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEPhantom
    {
        public class SigmaSquadronPilot : TIEPhantom
        {
            public SigmaSquadronPilot() : base()
            {
                PilotName = "Sigma Squadron Pilot";
                ImageUrl = "https://vignette2.wikia.nocookie.net/xwing-miniatures/images/d/d8/Sigma-squadron-pilot.png";
                PilotSkill = 3;
                Cost = 25;
            }
        }
    }
}

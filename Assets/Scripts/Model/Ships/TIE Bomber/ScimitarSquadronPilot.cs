using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEBomber
    {
        public class ScimitarSquadronPilot : TIEBomber
        {
            public ScimitarSquadronPilot() : base()
            {
                PilotName = "Scimitar Squadron Pilot";
                ImageUrl = "https://vignette3.wikia.nocookie.net/xwing-miniatures/images/a/a3/Scimitar-squadron-pilot.png";
                PilotSkill = 2;
                Cost = 16;
            }
        }
    }
}

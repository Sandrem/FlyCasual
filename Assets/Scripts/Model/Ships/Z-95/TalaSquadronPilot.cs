using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace Z95
    {
        public class TalaSquadronPilot : Z95
        {
            public TalaSquadronPilot() : base()
            {
                PilotName = "Tala Squadron Pilot";
                ImageUrl = "http://vignette1.wikia.nocookie.net/xwing-miniatures/images/b/b0/Tala-squadron-pilot.png";
                PilotSkill = 4;
                Cost = 13;

                faction = Faction.Rebel;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace EWing
    {
        public class KnaveSquadronPilot : EWing
        {
            public KnaveSquadronPilot() : base()
            {
                PilotName = "Knave Squadron Pilot";
                ImageUrl = "https://vignette4.wikia.nocookie.net/xwing-miniatures/images/6/61/Knave-squadron-pilot.png";
                PilotSkill = 1;
                Cost = 27;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace AWing
    {
        public class PrototypePilot : AWing
        {
            public PrototypePilot() : base()
            {
                PilotName = "Prototype Pilot";
                ImageUrl = "https://vignette2.wikia.nocookie.net/xwing-miniatures/images/f/fc/Prototype_Pilot.png";
                PilotSkill = 1;
                Cost = 17;
            }
        }
    }
}

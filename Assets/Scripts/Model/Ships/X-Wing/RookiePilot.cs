using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace XWing
    {
        public class RookiePilot : XWing
        {
            public RookiePilot() : base()
            {
                PilotName = "Rookie Pilot";
                PilotSkill = 2;
                Cost = 21;
            }
        }
    }
}

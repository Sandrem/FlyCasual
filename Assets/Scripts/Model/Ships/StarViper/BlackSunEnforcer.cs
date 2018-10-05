using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace StarViper
    {
        public class BlackSunEnforcer : StarViper, ISecondEditionPilot
        {
            public BlackSunEnforcer() : base()
            {
                PilotName = "Black Sun Enforcer";
                PilotSkill = 1;
                Cost = 25;
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 2;
                Cost = 46;

                SEImageNumber = 182;
            }
        }
    }
}

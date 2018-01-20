using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace VT49Decimator
    {
        public class PatrolLeader : VT49Decimator
        {
            public PatrolLeader() : base()
            {
                PilotName = "Patrol Leader";
                PilotSkill = 3;
                Cost = 40;
            }
        }
    }
}

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
                PilotSkill = 1;
                Cost = 17;

                SkinName = "Blue";
            }
        }
    }
}

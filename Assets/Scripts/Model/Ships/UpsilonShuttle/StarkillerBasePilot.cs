using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace UpsilonShuttle
    {
        public class StarkillerBasePilot : UpsilonShuttle
        {
            public StarkillerBasePilot() : base()
            {
                PilotName = "Starkiller Base Pilot";
                PilotSkill = 2;
                Cost = 30;
            }
        }
    }
}

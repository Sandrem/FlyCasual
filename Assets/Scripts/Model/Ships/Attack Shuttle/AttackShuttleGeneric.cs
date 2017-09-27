using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace AttackShuttle
    {
        public class AttackShuttleGeneric : AttackShuttle
        {
            public AttackShuttleGeneric() : base()
            {
                PilotName = "Attack Shuttle Generic";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Rebel%20Alliance/Attack%20Shuttle/zeb-orrelios.png";
                PilotSkill = 3;
                Cost = 18;
            }
        }
    }
}

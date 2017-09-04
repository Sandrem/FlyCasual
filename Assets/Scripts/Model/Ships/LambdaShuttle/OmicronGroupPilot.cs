using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace LambdaShuttle
    {
        public class OmicronGroupPilot : LambdaShuttle
        {
            public OmicronGroupPilot() : base()
            {
                PilotName = "Omicron Group Pilot";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Galactic%20Empire/Lambda-class%20Shuttle/omicron-group-pilot.png";
                PilotSkill = 2;
                Cost = 21;
            }
        }
    }
}

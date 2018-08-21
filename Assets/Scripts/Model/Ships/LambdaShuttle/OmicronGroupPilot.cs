using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace LambdaShuttle
    {
        public class OmicronGroupPilot : LambdaShuttle, ISecondEditionPilot
        {
            public OmicronGroupPilot() : base()
            {
                PilotName = "Omicron Group Pilot";
                PilotSkill = 2;
                Cost = 21;
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 1;
                Cost = 43;
            }
        }
    }
}

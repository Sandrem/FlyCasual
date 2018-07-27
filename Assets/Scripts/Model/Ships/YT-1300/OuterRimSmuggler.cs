using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace YT1300
    {
        public class OuterRimSmuggler : YT1300, ISecondEditionPilot
        {
            public OuterRimSmuggler() : base()
            {
                PilotName = "Outer Rim Smuggler";
                PilotSkill = 1;
                Cost = 27;
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 1;
                Cost = 78;
            }
        }
    }
}

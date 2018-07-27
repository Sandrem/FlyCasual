using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace YT2400
    {
        public class WildSpaceFringer : YT2400, ISecondEditionPilot
        {
            public WildSpaceFringer() : base()
            {
                PilotName = "Wild Space Fringer";
                PilotSkill = 2;
                Cost = 30;
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 1;
                Cost = 88;
            }
        }
    }
}

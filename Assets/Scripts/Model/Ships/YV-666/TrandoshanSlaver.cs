using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace YV666
    {
        public class TrandoshanSlaver : YV666, ISecondEditionPilot
        {
            public TrandoshanSlaver() : base()
            {
                PilotName = "Trandoshan Slaver";
                PilotSkill = 2;
                Cost = 29;
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 2;
            }
        }
    }
}

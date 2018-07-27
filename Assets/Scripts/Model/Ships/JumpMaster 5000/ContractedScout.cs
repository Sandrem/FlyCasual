using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace JumpMaster5000
    {
        public class ContractedScout : JumpMaster5000, ISecondEditionPilot
        {
            public ContractedScout() : base()
            {
                PilotName = "Contracted Scout";
                PilotSkill = 3;
                Cost = 25;
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 2;
                Cost = 52;
            }
        }
    }
}

using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEAggressor
    {
        public class SienarSpecialist : TIEAggressor, ISecondEditionPilot
        {
            public SienarSpecialist() : base()
            {
                PilotName = "Sienar Specialist";
                PilotSkill = 2;
                Cost = 17;
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 2;
            }
        }
    }
}

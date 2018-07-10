using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace M3AScyk
    {
        public class CartelSpacer : M3AScyk, ISecondEditionPilot
        {
            public CartelSpacer() : base()
            {
                PilotName = "Cartel Spacer";
                PilotSkill = 2;
                Cost = 14;
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 1;
            }
        }
    }
}

using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace Vcx100
    {
        public class LothalRebel : Vcx100, ISecondEditionPilot
        {
            public LothalRebel() : base()
            {
                PilotName = "Lothal Rebel";
                PilotSkill = 3;
                Cost = 35;
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 2;
                Cost = 70;

                SEImageNumber = 76;
            }
        }
    }
}

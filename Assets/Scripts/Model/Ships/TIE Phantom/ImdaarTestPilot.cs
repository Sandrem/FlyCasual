using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEPhantom
    {
        public class ImdaarTestPilot : TIEPhantom, ISecondEditionPilot
        {
            public ImdaarTestPilot() : base()
            {
                PilotName = "Imdaar Test Pilot";
                PilotSkill = 3;
                Cost = 25;
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 3;
            }
        }
    }
}

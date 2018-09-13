using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEReaper
    {
        public class ScarifBasePilot : TIEReaper, ISecondEditionPilot
        {
            public ScarifBasePilot() : base()
            {
                PilotName = "Scarif Base Pilot";
                PilotSkill = 1;
                Cost = 22;
            }

            public void AdaptPilotToSecondEdition()
            {
                Cost = 41;

                SEImageNumber = 116;
            }
        }
    }
}

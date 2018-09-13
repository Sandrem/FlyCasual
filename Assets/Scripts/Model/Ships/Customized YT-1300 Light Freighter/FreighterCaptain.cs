using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace ScumYT1300
    {
        public class FreighterCaptain : ScumYT1300, ISecondEditionPilot
        {
            public FreighterCaptain() : base()
            {
                PilotName = "Freighter Captain";
                PilotSkill = 1;
                Cost = 46;

                SEImageNumber = 225;
            }

            public void AdaptPilotToSecondEdition()
            {
                // Not required
            }
        }
    }
}

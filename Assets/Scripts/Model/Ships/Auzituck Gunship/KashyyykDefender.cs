using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace AuzituckGunship
    {
        public class KashyyykDefender : AuzituckGunship, ISecondEditionPilot
        {
            public KashyyykDefender() : base()
            {
                PilotName = "Kashyyyk Defender";
                PilotSkill = 1;
                Cost = 24;
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 1;
                Cost = 46;

                SEImageNumber = 33;
            }
        }
    }
}

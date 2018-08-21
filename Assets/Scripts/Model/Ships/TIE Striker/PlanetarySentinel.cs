using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEStriker
    {
        public class PlanetarySentinel : TIEStriker, ISecondEditionPilot
        {
            public PlanetarySentinel() : base()
            {
                PilotName = "Planetary Sentinel";
                PilotSkill = 1;
                Cost = 17;

                PilotRuleType = typeof(SecondEdition);
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 1;
                Cost = 34;
            }
        }
    }
}

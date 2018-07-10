using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace UWing
    {
        public class BlueSquadronScout : UWing, ISecondEditionPilot
        {
            public BlueSquadronScout() : base()
            {
                PilotName = "Blue Squadron Scout";
                PilotSkill = 2;
                Cost = 23;

                PilotRuleType = typeof(SecondEdition);
            }

            public void AdaptPilotToSecondEdition()
            {
                // Not requred
            }
        }
    }
}

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
                Cost = 43;

                PilotRuleType = typeof(SecondEdition);

                SEImageNumber = 60;
            }

            public void AdaptPilotToSecondEdition()
            {
                // Not requred
            }
        }
    }
}

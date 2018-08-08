using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace BWing
    {
        public class BlueSquadronPilot : BWing, ISecondEditionPilot
        {
            public BlueSquadronPilot() : base()
            {
                PilotName = "Blue Squadron Pilot";
                PilotSkill = 2;
                Cost = 22;

                SkinName = "Blue";
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 2;
                Cost = 42;
            }
        }
    }
}

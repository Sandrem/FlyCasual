using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace BWing
    {
        public class BladeSquadronVeteran : BWing, ISecondEditionPilot
        {
            public BladeSquadronVeteran() : base()
            {
                PilotName = "Blade Squadron Veteran";
                PilotSkill = 3;
                Cost = 24;

                SkinName = "Red";

                PilotRuleType = typeof(SecondEdition);
            }

            public void AdaptPilotToSecondEdition()
            {
                //Not required
            }
        }
    }
}

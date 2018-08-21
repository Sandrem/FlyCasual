using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace HWK290
    {
        public class RebelScout : HWK290, ISecondEditionPilot
        {
            public RebelScout() : base()
            {
                PilotName = "Rebel Scout";
                PilotSkill = 2;
                Cost = 32;

                faction = Faction.Rebel;

                PilotRuleType = typeof(SecondEdition);
            }

            public void AdaptPilotToSecondEdition()
            {
                //Not required
            }
        }
    }
}

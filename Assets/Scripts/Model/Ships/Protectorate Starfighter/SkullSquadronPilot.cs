using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace ProtectorateStarfighter
    {
        public class SkullSquadronPilot : ProtectorateStarfighter, ISecondEditionPilot
        {
            public SkullSquadronPilot() : base()
            {
                PilotName = "Skull Squadron Pilot";
                PilotSkill = 4;
                Cost = 20;

                PilotRuleType = typeof(SecondEdition);
            }

            public void AdaptPilotToSecondEdition()
            {
                // No adaptation is needed
            }
        }
    }
}

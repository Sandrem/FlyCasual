using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace EWing
    {
        public class KnaveSquadronEscort : EWing, ISecondEditionPilot
        {
            public KnaveSquadronEscort() : base()
            {
                PilotName = "Knave Squadron Escort";
                PilotSkill = 2;
                Cost = 27;

                PilotRuleType = typeof(SecondEdition);
            }

            public void AdaptPilotToSecondEdition()
            {
                //Not required
            }
        }
    }
}

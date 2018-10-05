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
                Cost = 61;

                PilotRuleType = typeof(SecondEdition);

                SEImageNumber = 53;
            }

            public void AdaptPilotToSecondEdition()
            {
                //Not required
            }
        }
    }
}

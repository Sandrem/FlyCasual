using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEDefender
    {
        public class OnyxSquadronAce : TIEDefender, ISecondEditionPilot
        {
            public OnyxSquadronAce() : base()
            {
                PilotName = "Onyx Squadron Ace";
                PilotSkill = 4;
                Cost = 32;

                PilotRuleType = typeof(SecondEdition);
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 4;
            }
        }
    }
}

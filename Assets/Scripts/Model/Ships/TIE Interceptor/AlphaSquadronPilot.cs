using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEInterceptor
    {
        public class AlphaSquadronPilot : TIEInterceptor, ISecondEditionPilot
        {
            public AlphaSquadronPilot() : base()
            {
                PilotName = "Alpha Squadron Pilot";
                PilotSkill = 1;
                Cost = 18;
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 1;
            }
        }
    }
}

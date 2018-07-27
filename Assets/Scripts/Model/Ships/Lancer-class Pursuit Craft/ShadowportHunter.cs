using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RuleSets;

namespace Ship
{
    namespace LancerClassPursuitCraft
    {
        public class ShadowportHunter : LancerClassPursuitCraft, ISecondEditionPilot
        {
            public ShadowportHunter() : base()
            {
                PilotName = "Shadowport Hunter";
                PilotSkill = 2;
                Cost = 33;
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 2;
                Cost = 64;
            }
        }
    }
}

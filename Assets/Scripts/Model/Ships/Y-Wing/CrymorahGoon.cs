using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace YWing
    {
        public class CrymorahGoon : YWing, ISecondEditionPilot
        {
            public CrymorahGoon() : base()
            {
                PilotName = "Crymorah Goon";
                PilotSkill = 1;
                Cost = 32;

                faction = Faction.Scum;

                SkinName = "Brown";

                PilotRuleType = typeof(SecondEdition);
            }

            public void AdaptPilotToSecondEdition()
            {
                //No adaptation is needed
            }
        }
    }
}

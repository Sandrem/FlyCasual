using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEAdvPrototype
    {
        public class Inquisitor : TIEAdvPrototype, ISecondEditionPilot
        {
            public Inquisitor() : base()
            {
                PilotName = "Inquisitor";
                PilotSkill = 3;
                Cost = 19;

                MaxForce = 1;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Force);

                PilotRuleType = typeof(SecondEdition);
            }

            public void AdaptPilotToSecondEdition()
            {
                //No adaptation is required
            }
        }
    }
}

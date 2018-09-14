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
                Cost = 40;

                MaxForce = 1;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Force);

                PilotRuleType = typeof(SecondEdition);

                SEImageNumber = 102;
            }

            public void AdaptPilotToSecondEdition()
            {
                //No adaptation is required
            }
        }
    }
}

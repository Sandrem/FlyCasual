using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEAdvPrototype
    {
        public class BaronOfTheEmpire : TIEAdvPrototype, ISecondEditionPilot
        {
            public BaronOfTheEmpire() : base()
            {
                PilotName = "Baron of the Empire";
                PilotSkill = 4;
                Cost = 19;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 3;
                Cost = 34;
            }
        }
    }
}

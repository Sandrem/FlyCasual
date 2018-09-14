using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace BWing
    {
        public class BladeSquadronVeteran : BWing, ISecondEditionPilot
        {
            public BladeSquadronVeteran() : base()
            {
                PilotName = "Blade Squadron Veteran";
                PilotSkill = 3;
                Cost = 44;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                SkinName = "Red";

                PilotRuleType = typeof(SecondEdition);

                SEImageNumber = 25;
            }

            public void AdaptPilotToSecondEdition()
            {
                //Not required
            }
        }
    }
}

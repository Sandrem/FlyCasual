using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace Kihraxz
    {
        public class BlackSunAce : Kihraxz, ISecondEditionPilot
        {
            public BlackSunAce() : base()
            {
                PilotName = "Black Sun Ace";
                PilotSkill = 5;
                Cost = 23;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                SkinName = "Black Sun (White)";
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 3;
                Cost = 42;
            }
        }
    }
}

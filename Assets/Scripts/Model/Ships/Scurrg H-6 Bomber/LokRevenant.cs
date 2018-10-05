using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace ScurrgH6Bomber
    {
        public class LokRevenant : ScurrgH6Bomber, ISecondEditionPilot
        {
            public LokRevenant() : base()
            {
                PilotName = "Lok Revenant";
                PilotSkill = 3;
                Cost = 26;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 2;
                Cost = 46;

                PrintedUpgradeIcons.Remove(Upgrade.UpgradeType.Elite);

                SEImageNumber = 206;
            }
        }
    }
}

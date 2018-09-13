using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace StarViper
    {
        public class BlackSunAssassin : StarViper, ISecondEditionPilot
        {
            public BlackSunAssassin() : base()
            {
                PilotName = "Black Sun Assassin";
                PilotSkill = 5;
                Cost = 28;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                SkinName = "Black Sun Assassin";
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 3;
                Cost = 48;

                SEImageNumber = 181;
            }
        }
    }
}

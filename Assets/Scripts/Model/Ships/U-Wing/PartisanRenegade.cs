using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace UWing
    {
        public class PartisanRenegade : UWing, ISecondEditionPilot
        {
            public PartisanRenegade() : base()
            {
                PilotName = "Partisan Renegade";
                PilotSkill = 1;
                Cost = 22;

                SkinName = "Partisan";
            }

            public void AdaptPilotToSecondEdition()
            {
                Cost = 43;
                PilotSkill = 1;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Illicit);

                SEImageNumber = 61;
            }
        }
    }
}

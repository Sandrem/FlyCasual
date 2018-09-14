using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace Ship
{
    namespace XWing
    {
        public class CavernAngelsZealot : XWing, ISecondEditionPilot
        {
            public CavernAngelsZealot() : base()
            {
                PilotName = "Cavern Angels Zealot";
                PilotSkill = 1;
                Cost = 22;

                SkinName = "Partisan";

                PrintedUpgradeIcons.Add(UpgradeType.Elite);
            }

            public void AdaptPilotToSecondEdition()
            {
                Cost = 41;
                PilotSkill = 1;

                PrintedUpgradeIcons.Remove(UpgradeType.Elite);
                PrintedUpgradeIcons.Add(UpgradeType.Illicit);

                SEImageNumber = 12;
            }
        }
    }
}

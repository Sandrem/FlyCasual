using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace M3AScyk
    {
        public class TansariiPointVeteran : M3AScyk, ISecondEditionPilot
        {
            public TansariiPointVeteran() : base()
            {
                PilotName = "Tansarii Point Veteran";
                PilotSkill = 5;
                Cost = 17;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                SkinName = "Serissu";
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 3;
            }
        }
    }
}

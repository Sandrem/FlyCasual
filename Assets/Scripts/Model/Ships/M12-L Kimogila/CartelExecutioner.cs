using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace M12LKimogila
    {
        public class CartelExecutioner : M12LKimogila, ISecondEditionPilot
        {
            public CartelExecutioner() : base()
            {
                PilotName = "Cartel Executioner";
                PilotSkill = 5;
                Cost = 24;

                SkinName = "Cartel Executioner";

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 3;
                Cost = 44;

                SEImageNumber = 209;
            }
        }
    }
}

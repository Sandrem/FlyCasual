using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIESF
    {
        public class OmegaSpecialist : TIESF
        {
            public OmegaSpecialist() : base()
            {
                PilotName = "Omega Specialist";
                PilotSkill = 5;
                Cost = 25;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
            }
        }
    }
}

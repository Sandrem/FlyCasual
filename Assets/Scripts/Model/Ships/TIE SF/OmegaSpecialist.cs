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
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/First%20Order/TIE-sf%20Fighter/omega-specialist.png";
                PilotSkill = 5;
                Cost = 25;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
            }
        }
    }
}

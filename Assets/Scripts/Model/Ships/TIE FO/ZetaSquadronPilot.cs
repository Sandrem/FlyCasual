using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEFO
    {
        public class ZetaSquadronPilot : TIEFO
        {
            public ZetaSquadronPilot() : base()
            {
                PilotName = "Zeta Squadron Pilot";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/First%20Order/TIE-fo%20Fighter/zeta-squadron-pilot.png";
                PilotSkill = 3;
                Cost = 16;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
            }
        }
    }
}

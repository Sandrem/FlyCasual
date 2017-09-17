using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEFO
    {
        public class OmegaSquadronPilot : TIEFO
        {
            public OmegaSquadronPilot() : base()
            {
                PilotName = "Omega Squadron Pilot";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/First%20Order/TIE-fo%20Fighter/omega-squadron-pilot.png";
                PilotSkill = 4;
                Cost = 17;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
            }
        }
    }
}

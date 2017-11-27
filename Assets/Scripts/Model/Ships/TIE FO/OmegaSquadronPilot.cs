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
                PilotSkill = 4;
                Cost = 17;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
            }
        }
    }
}

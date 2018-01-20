using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace AWing
    {
        public class GreenSquadronPilot : AWing
        {
            public GreenSquadronPilot() : base()
            {
                PilotName = "Green Squadron Pilot";
                PilotSkill = 3;
                Cost = 19;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                SkinName = "Green";
            }
        }
    }
}

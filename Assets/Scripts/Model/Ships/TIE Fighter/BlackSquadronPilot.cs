using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEFighter
    {
        public class BlackSquadronPilot: TIEFighter
        {
            public BlackSquadronPilot() : base()
            {
                PilotName = "Black Squadron Pilot";
                PilotSkill = 4;
                Cost = 14;
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
            }
        }
    }
}

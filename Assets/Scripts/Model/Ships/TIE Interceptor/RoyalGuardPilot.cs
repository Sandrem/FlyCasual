using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEInterceptor
    {
        public class RoyalGuardPilot : TIEInterceptor
        {
            public RoyalGuardPilot() : base()
            {
                PilotName = "Royal Guard Pilot";
                PilotSkill = 6;
                Cost = 22;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                SkinName = "Royal Guard";
            }
        }
    }
}

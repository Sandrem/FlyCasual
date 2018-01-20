using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEInterceptor
    {
        public class SaberSquadronPilot : TIEInterceptor
        {
            public SaberSquadronPilot() : base()
            {
                PilotName = "Saber Squadron Pilot";
                PilotSkill = 4;
                Cost = 21;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                SkinName = "Red Stripes";
            }
        }
    }
}

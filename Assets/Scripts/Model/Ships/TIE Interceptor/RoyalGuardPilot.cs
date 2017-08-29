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
                ImageUrl = "https://vignette1.wikia.nocookie.net/xwing-miniatures/images/d/d0/Royal_Guard_TIE.png";
                PilotSkill = 6;
                Cost = 22;

                AddUpgradeSlot(Upgrade.UpgradeType.Elite);
            }
        }
    }
}

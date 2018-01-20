using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace AuzituckGunship
    {
        public class WookieeLiberator : AuzituckGunship
        {
            public WookieeLiberator() : base()
            {
                PilotName = "Wookiee Liberator";
                PilotSkill = 3;
                Cost = 26;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
            }
        }
    }
}

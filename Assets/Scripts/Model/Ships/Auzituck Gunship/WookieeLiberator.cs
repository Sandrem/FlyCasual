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
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Rebel%20Alliance/Auzituck%20Gunship/wookiee-liberator.png";
                PilotSkill = 3;
                Cost = 26;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
            }
        }
    }
}

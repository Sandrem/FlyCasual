using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace YT1300
    {
        public class HanSolo : YT1300
        {
            public HanSolo() : base()
            {
                PilotName = "Han Solo";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Rebel%20Alliance/YT-1300/han-solo.png";
                PilotSkill = 9;
                Cost = 46;

                IsUnique = true;

                Firepower = 3;
                MaxHull = 8;
                MaxShields = 5;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Missile);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace YT1300
    {
        public class Chewbacca : YT1300
        {
            public Chewbacca() : base()
            {
                PilotName = "Chewbacca";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Rebel%20Alliance/YT-1300/chewbacca.png";
                PilotSkill = 5;
                Cost = 42;

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

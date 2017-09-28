using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace YT1300
    {
        public class LandoCalrissian : YT1300
        {
            public LandoCalrissian() : base()
            {
                PilotName = "Lando Calrissian";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Rebel%20Alliance/YT-1300/lando-calrissian.png";
                PilotSkill = 7;
                Cost = 44;

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace Z95
    {
        public class BlackSunSoldier : Z95
        {
            public BlackSunSoldier() : base()
            {
                PilotName = "Black Sun Soldier";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Scum%20and%20Villainy/Z-95%20Headhunter/black-sun-soldier.png";
                PilotSkill = 3;
                Cost = 13;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Illicit);

                SkinName = "Black Sun";

                faction = Faction.Scum;
            }
        }
    }
}

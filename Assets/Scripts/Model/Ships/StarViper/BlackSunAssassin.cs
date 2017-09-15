using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace StarViper
    {
        public class BlackSunAssassin : StarViper
        {
            public BlackSunAssassin() : base()
            {
                PilotName = "Black Sun Assassin";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Scum%20and%20Villainy/StarViper/black-sun-enforcer.png";
                PilotSkill = 5;
                Cost = 28;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                SkinName = "Black Sun Assassin";
            }
        }
    }
}

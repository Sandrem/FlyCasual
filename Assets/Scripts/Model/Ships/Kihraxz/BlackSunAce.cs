using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace Kihraxz
    {
        public class BlackSunAce : Kihraxz
        {
            public BlackSunAce() : base()
            {
                PilotName = "Black Sun Ace";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Scum%20and%20Villainy/Kihraxz%20Fighter/black-sun-ace.png";
                PilotSkill = 5;
                Cost = 23;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                SkinName = "Black Sun (White)";
            }
        }
    }
}

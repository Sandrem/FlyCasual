using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace G1AStarfighter
    {
        public class GandFindsman : G1AStarfighter
        {
            public GandFindsman() : base()
            {
                PilotName = "Gand Findsman";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Scum%20and%20Villainy/G-1A%20Starfighter/gand-findsman.png";
                PilotSkill = 5;
                Cost = 25;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
            }
        }
    }
}

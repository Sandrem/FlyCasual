using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace Quadjumper
    {
        public class JakkuGunrunner : Quadjumper
        {
            public JakkuGunrunner() : base()
            {
                PilotName = "Jakku Gunrunner";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Scum%20and%20Villainy/Quadjumper/jakku-gunrunner.png";
                PilotSkill = 1;
                Cost = 15;
            }
        }
    }
}

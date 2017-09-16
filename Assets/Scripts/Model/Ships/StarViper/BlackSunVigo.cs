using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace StarViper
    {
        public class BlackSunVigo : StarViper
        {
            public BlackSunVigo() : base()
            {
                PilotName = "Black Sun Vigo";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Scum%20and%20Villainy/StarViper/black-sun-vigo.png";
                PilotSkill = 3;
                Cost = 27;

                SkinName = "Black Sun Vigo";
            }
        }
    }
}

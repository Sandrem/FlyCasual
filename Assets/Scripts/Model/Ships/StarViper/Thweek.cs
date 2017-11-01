using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace StarViper
    {
        public class Thweek : StarViper
        {
            public Thweek() : base()
            {
                PilotName = "Thweek";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Scum%20and%20Villainy/StarViper/thweek.png";
                PilotSkill = 4;
                Cost = 28;

                IsUnique = true;
            }
        }
    }
}

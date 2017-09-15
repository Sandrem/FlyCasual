using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace Agressor
    {
        public class IG88Generic : Agressor
        {
            public IG88Generic() : base()
            {
                PilotName = "IG-88 Generic";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Scum%20and%20Villainy/Aggressor/ig-88a.png";
                PilotSkill = 6;
                Cost = 36;
            }
        }
    }
}

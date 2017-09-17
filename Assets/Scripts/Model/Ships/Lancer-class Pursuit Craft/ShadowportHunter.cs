using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace LancerClassPursuitCraft
    {
        public class ShadowportHunter : LancerClassPursuitCraft
        {
            public ShadowportHunter() : base()
            {
                PilotName = "Shadowport Hunter";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Scum%20and%20Villainy/Lancer-class%20Pursuit%20Craft/shadowport-hunter.png";
                PilotSkill = 2;
                Cost = 33;
            }
        }
    }
}

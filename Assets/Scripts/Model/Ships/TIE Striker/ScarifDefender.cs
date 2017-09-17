using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEStriker
    {
        public class ScarifDefender : TIEStriker
        {
            public ScarifDefender() : base()
            {
                PilotName = "Scarif Defender";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Galactic%20Empire/TIE%20Striker/scarif-defender.png";
                PilotSkill = 3;
                Cost = 18;
            }
        }
    }
}

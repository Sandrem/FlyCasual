using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEPunisher
    {
        public class BlackEightSqPilot : TIEPunisher
        {
            public BlackEightSqPilot() : base()
            {
                PilotName = "Black Eight Sq. Pilot";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Galactic%20Empire/TIE%20Punisher/black-eight-squadron-pilot.png";
                PilotSkill = 4;
                Cost = 23;
            }
        }
    }
}

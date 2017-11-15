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
                ImageUrl = ImageUrls.GetImageUrl(this, "black-eight-squadron-pilot.png");
                PilotSkill = 4;
                Cost = 23;
            }
        }
    }
}

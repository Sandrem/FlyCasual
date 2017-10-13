using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEStriker
    {
        public class ImperialTrainee : TIEStriker
        {
            public ImperialTrainee() : base()
            {
                PilotName = "Imperial Trainee";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Galactic%20Empire/TIE%20Striker/imperial-trainee.png";
                PilotSkill = 1;
                Cost = 17;
            }
        }
    }
}

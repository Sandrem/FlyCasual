using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace ProtectorateStarfighter
    {
        public class ZealousRecruit : ProtectorateStarfighter
        {
            public ZealousRecruit() : base()
            {
                PilotName = "Zealous Recruit";
                PilotSkill = 1;
                Cost = 20;
            }
        }
    }
}

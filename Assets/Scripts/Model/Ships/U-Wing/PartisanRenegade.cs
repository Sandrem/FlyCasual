using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace UWing
    {
        public class PartisanRenegade : UWing
        {
            public PartisanRenegade() : base()
            {
                PilotName = "Partisan Renegade";
                PilotSkill = 1;
                Cost = 21;

                SkinName = "Partisan";
            }
        }
    }
}

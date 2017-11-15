using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEFighter
    {
        public class ZebOrrelios : TIEFighter
        {
            public ZebOrrelios() : base()
            {
                PilotName = "Rebel TIE Generic";
                PilotSkill = 4;
                Cost = 14;

                faction = Faction.Rebel;
            }
        }
    }
}

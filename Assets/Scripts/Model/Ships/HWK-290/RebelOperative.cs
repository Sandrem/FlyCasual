using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace HWK290
    {
        public class RebelOperative : HWK290
        {
            public RebelOperative() : base()
            {
                PilotName = "Rebel Operative";
                PilotSkill = 2;
                Cost = 16;

                faction = Faction.Rebel;
            }
        }
    }
}

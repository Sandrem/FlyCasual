using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace XWing
    {
        public class BiggsDarklighter : XWing
        {
            public BiggsDarklighter(Player playerNo, int shipId, Vector3 position) : base(playerNo, shipId, position)
            {
                PilotName = "Biggs Darklighter";
                isUnique = true;
                PilotSkill = 5;
            }
        }
    }
}

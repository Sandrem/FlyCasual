using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace XWing
    {
        public class RookiePilot : XWing
        {
            public RookiePilot(Player playerNo, int shipId, Vector3 position) : base(playerNo, shipId, position)
            {
                PilotName = "Rookie Pilot";
                PilotSkill = 2;
            }
        }
    }
}

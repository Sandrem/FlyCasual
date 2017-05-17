using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace XWing
    {
        public class RookiePilot : XWing
        {
            public RookiePilot(Players.GenericPlayer owner, int shipId, Vector3 position) : base(owner, shipId, position)
            {
                PilotName = "Rookie Pilot";
                PilotSkill = 2;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace XWing
    {
        public class RedSquadronPilot : XWing
        {
            public RedSquadronPilot(Player playerNo, int shipId, Vector3 position) : base(playerNo, shipId, position)
            {
                PilotName = "Red Squadron Pilot";
                PilotSkill = 4;
            }
        }
    }
}

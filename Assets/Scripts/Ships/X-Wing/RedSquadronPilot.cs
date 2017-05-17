using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace XWing
    {
        public class RedSquadronPilot : XWing
        {
            public RedSquadronPilot(Players.GenericPlayer owner, int shipId, Vector3 position) : base(owner, shipId, position)
            {
                PilotName = "Red Squadron Pilot";
                PilotSkill = 4;
            }
        }
    }
}

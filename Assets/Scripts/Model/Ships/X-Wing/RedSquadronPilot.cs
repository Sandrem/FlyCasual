using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace XWing
    {
        public class RedSquadronPilot : XWing
        {
            public RedSquadronPilot(Players.PlayerNo playerNo, int shipId, Vector3 position) : base(playerNo, shipId, position)
            {
                PilotName = "Red Squadron Pilot";
                ImageUrl = "https://vignette4.wikia.nocookie.net/xwing-miniatures/images/3/39/Red-squadron-pilot.png";
                PilotSkill = 4;

                InitializePilot();
            }
        }
    }
}

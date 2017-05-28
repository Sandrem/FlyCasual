using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace XWing
    {
        public class RookiePilot : XWing
        {
            public RookiePilot(Players.PlayerNo playerNo, int shipId, Vector3 position) : base(playerNo, shipId, position)
            {
                PilotName = "Rookie Pilot";
                ImageUrl = "https://vignette1.wikia.nocookie.net/xwing-miniatures/images/c/cc/Rookie-pilot.png";
                PilotSkill = 2;

                InitializePilot();
            }
        }
    }
}

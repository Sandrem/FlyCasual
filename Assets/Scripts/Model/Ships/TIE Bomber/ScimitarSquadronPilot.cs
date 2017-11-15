using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEBomber
    {
        public class ScimitarSquadronPilot : TIEBomber
        {
            public ScimitarSquadronPilot() : base()
            {
                PilotName = "Scimitar Squadron Pilot";
                PilotSkill = 2;
                Cost = 16;
            }
        }
    }
}

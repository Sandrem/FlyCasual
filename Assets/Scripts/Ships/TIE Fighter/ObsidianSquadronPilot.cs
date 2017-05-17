using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEFighter
    {
        public class ObsidianSquadronPilot: TIEFighter
        {
            public ObsidianSquadronPilot(Players.GenericPlayer owner, int shipId, Vector3 position) : base(owner, shipId, position)
            {
                PilotName = "Obsidian Squadron Pilot";
                PilotSkill = 3;
            }
        }
    }
}

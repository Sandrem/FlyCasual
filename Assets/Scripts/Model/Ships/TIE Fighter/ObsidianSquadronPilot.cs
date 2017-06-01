using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEFighter
    {
        public class ObsidianSquadronPilot: TIEFighter
        {
            public ObsidianSquadronPilot() : base()
            {
                PilotName = "Obsidian Squadron Pilot";
                ImageUrl = "https://vignette3.wikia.nocookie.net/xwing-miniatures/images/e/ef/Obsidian-squadron-pilot.png";
                PilotSkill = 3;
            }
        }
    }
}

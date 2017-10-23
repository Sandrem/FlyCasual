using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace AlphaClassStarWing
    {
        public class NuSquadronPilot : AlphaClassStarWing
        {
            public NuSquadronPilot() : base()
            {
                PilotName = "Nu Squadron Pilot";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Galactic%20Empire/Alpha-class%20Star%20Wing/nu-squadron-pilot.png";
                PilotSkill = 2;
                Cost = 18;
            }
        }
    }
}

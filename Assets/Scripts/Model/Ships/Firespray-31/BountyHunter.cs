using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace Firespray31
    {
        public class BountyHunter : Firespray31
        {
            public BountyHunter() : base()
            {
                PilotName = "Bounty Hunter";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Galactic%20Empire/Firespray-31/bounty-hunter.png";
                PilotSkill = 3;
                Cost = 33;

                SkinName = "Bounty Hunter";

                faction = Faction.Empire;
            }
        }
    }
}

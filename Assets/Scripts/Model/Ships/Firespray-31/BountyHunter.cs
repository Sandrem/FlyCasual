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
                PilotSkill = 3;
                Cost = 33;

                SkinName = "Bounty Hunter";

                faction = Faction.Imperial;
            }
        }
    }
}

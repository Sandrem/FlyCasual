using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace Z95
    {
        public class TalaSquadronPilot : Z95
        {
            public TalaSquadronPilot() : base()
            {
                PilotName = "Tala Squadron Pilot";
                PilotSkill = 4;
                Cost = 13;

                faction = Faction.Rebel;

                SkinName = "Tala Squadron";
            }
        }
    }
}

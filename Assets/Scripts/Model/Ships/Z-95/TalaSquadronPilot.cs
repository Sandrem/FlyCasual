using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace Z95
    {
        public class TalaSquadronPilot : Z95, ISecondEditionPilot
        {
            public TalaSquadronPilot() : base()
            {
                PilotName = "Tala Squadron Pilot";
                PilotSkill = 4;
                Cost = 13;

                faction = Faction.Rebel;

                //SkinName = "Tala Squadron";
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 2;
            }
        }
    }
}

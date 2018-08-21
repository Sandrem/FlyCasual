using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace AlphaClassStarWing
    {
        public class NuSquadronPilot : AlphaClassStarWing, ISecondEditionPilot
        {
            public NuSquadronPilot() : base()
            {
                PilotName = "Nu Squadron Pilot";
                PilotSkill = 2;
                Cost = 18;
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 2;
                Cost = 35;
            }
        }
    }
}

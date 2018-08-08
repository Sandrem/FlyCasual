using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEBomber
    {
        public class ScimitarSquadronPilot : TIEBomber, ISecondEditionPilot
        {
            public ScimitarSquadronPilot() : base()
            {
                PilotName = "Scimitar Squadron Pilot";
                PilotSkill = 2;
                Cost = 16;
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 2;
                Cost = 28;
            }
        }
    }
}

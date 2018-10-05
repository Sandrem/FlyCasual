using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEDefender
    {
        public class DeltaSquadronPilot : TIEDefender, ISecondEditionPilot
        {
            public DeltaSquadronPilot() : base()
            {
                PilotName = "Delta Squadron Pilot";
                PilotSkill = 1;
                Cost = 30;
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 1;
                Cost = 72;

                SEImageNumber = 126;
            }
        }
    }
}

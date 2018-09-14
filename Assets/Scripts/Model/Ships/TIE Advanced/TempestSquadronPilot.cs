using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEAdvanced
    {
        public class TempestSquadronPilot : TIEAdvanced, ISecondEditionPilot
        {
            public TempestSquadronPilot() : base()
            {
                PilotName = "Tempest Squadron Pilot";
                PilotSkill = 2;
                Cost = 21;
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 2;
                Cost = 41;

                SEImageNumber = 98;
            }
        }
    }
}

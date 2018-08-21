using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEPunisher
    {
        public class CutlassSquadronPilot : TIEPunisher, ISecondEditionPilot
        {
            public CutlassSquadronPilot() : base()
            {
                PilotName = "Cutlass Squadron Pilot";
                PilotSkill = 2;
                Cost = 21;
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 2;
                Cost = 36;
            }
        }
    }
}

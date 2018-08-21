using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace KWing
    {
        public class WardenSquadronPilot : KWing, ISecondEditionPilot
        {
            public WardenSquadronPilot() : base()
            {
                PilotName = "Warden Squadron Pilot";
                PilotSkill = 2;
                Cost = 23;
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 2;
                Cost = 40;
            }
        }
    }
}

using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEPhantom
    {
        public class SigmaSquadronAce : TIEPhantom, ISecondEditionPilot
        {
            public SigmaSquadronAce() : base()
            {
                PilotName = "Sigma Squadron Ace";
                PilotSkill = 4;
                Cost = 25;
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 4;
            }
        }
    }
}

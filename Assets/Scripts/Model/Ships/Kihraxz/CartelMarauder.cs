using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace Kihraxz
    {
        public class CartelMarauder : Kihraxz, ISecondEditionPilot
        {
            public CartelMarauder() : base()
            {
                PilotName = "Cartel Marauder";
                PilotSkill = 2;
                Cost = 20;
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 2;
                Cost = 40;

                SEImageNumber = 196;
            }
        }
    }
}

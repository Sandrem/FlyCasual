using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace Quadjumper
    {
        public class JakkuGunrunner : Quadjumper, ISecondEditionPilot
        {
            public JakkuGunrunner() : base()
            {
                PilotName = "Jakku Gunrunner";
                PilotSkill = 1;
                Cost = 15;
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 1;
                Cost = 28;

                IsHidden = true;
            }
        }
    }
}

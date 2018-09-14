using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using RuleSets;

namespace Ship
{
    namespace EscapeCraft
    {
        public class AutopilotDrone : EscapeCraft, ISecondEditionPilot
        {
            public AutopilotDrone() : base()
            {
                PilotName = "Autopilot Drone";
                PilotSkill = 1;
                Cost = 12;

                IsUnique = true;

                // TODO
                // PilotAbilities.Add(new Abilities.ZebOrreliosPilotAbility());

                SEImageNumber = 229;
            }

            public void AdaptPilotToSecondEdition()
            {
                // Not required
            }
        }
    }
}
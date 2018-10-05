using Movement;
using RuleSets;
using Ship;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace ScumYT1300
    {
        public class L337 : ScumYT1300, ISecondEditionPilot
        {
            public L337() : base()
            {
                PilotName = "L3-37";
                PilotSkill = 2;
                Cost = 47;

                IsUnique = true;

                PilotAbilities.Add(new Abilities.SecondEdition.L337Ability());

                SEImageNumber = 224;
            }

            public void AdaptPilotToSecondEdition()
            {
                // Not required
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class L337Ability : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGetManeuverColorDecreaseComplexity += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGetManeuverColorDecreaseComplexity -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship, ref MovementStruct movement)
        {
            if (HostShip.Shields == 0 && movement.Bearing == ManeuverBearing.Bank)
            {
                movement.ColorComplexity = GenericMovement.ReduceComplexity(movement.ColorComplexity);
            }
        }
    }
}

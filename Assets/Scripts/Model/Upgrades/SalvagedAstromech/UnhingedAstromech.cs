using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Abilities;
using System;

namespace UpgradesList
{
    public class UnhingedAstromech : GenericUpgrade
    {
        public UnhingedAstromech() : base()
        {
            Types.Add(UpgradeType.SalvagedAstromech);
            Name = "Unhinged Astromech";
            Cost = 1;

            UpgradeAbilities.Add(new UnhingedAstromechAbility());
        }
    }
}

namespace Abilities
{
    public class UnhingedAstromechAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGetManeuverColorDecreaseComplexity += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGetManeuverColorDecreaseComplexity -= CheckAbility;
        }

        private void CheckAbility(Ship.GenericShip ship, ref Movement.MovementStruct movement)
        {
            if (movement.ColorComplexity != Movement.MovementComplexity.None)
            {
                if (movement.Speed == Movement.ManeuverSpeed.Speed3)
                {
                    movement.ColorComplexity = Movement.MovementComplexity.Easy;
                }
            }
        }

    }
}
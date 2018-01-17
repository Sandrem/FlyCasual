using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{

    public class UnhingedAstromech : GenericUpgrade
    {

        public UnhingedAstromech() : base()
        {
            Types.Add(UpgradeType.SalvagedAstromech);
            Name = "Unhinged Astromech";
            Cost = 1;
        }

        public override void AttachToShip(Ship.GenericShip host)
        {
            base.AttachToShip(host);

            host.AfterGetManeuverColorDecreaseComplexity += UnhingedAstromechAbility;
        }

        private void UnhingedAstromechAbility(Ship.GenericShip ship, ref Movement.MovementStruct movement)
        {
            if (movement.ColorComplexity != Movement.ManeuverColor.None)
            {
                if (movement.Speed == Movement.ManeuverSpeed.Speed3)
                {
                    movement.ColorComplexity = Movement.ManeuverColor.Green;
                }
            }
        }

    }

}

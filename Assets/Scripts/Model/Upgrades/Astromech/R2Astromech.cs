using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Abilities;
using RuleSets;

namespace UpgradesList
{

    public class R2Astromech : GenericUpgrade, ISecondEditionUpgrade
    {

        public R2Astromech() : base()
        {
            Types.Add(UpgradeType.Astromech);
            Name = "R2 Astromech";
            Cost = 1;

            UpgradeAbilities.Add(new R2AstromechAbility());
        }

        public void AdaptUpgradeToSecondEdition()
        {
            MaxCharges = 2;
        }

    }

}

namespace Abilities
{
    public class R2AstromechAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGetManeuverColorDecreaseComplexity += CheckR2AstromechAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGetManeuverColorDecreaseComplexity -= CheckR2AstromechAbility;
        }

        private void CheckR2AstromechAbility(Ship.GenericShip ship, ref Movement.MovementStruct movement)
        {
            if (movement.ColorComplexity != Movement.MovementComplexity.None)
            {
                if ((movement.Speed == Movement.ManeuverSpeed.Speed1) || (movement.Speed == Movement.ManeuverSpeed.Speed2))
                {
                    movement.ColorComplexity = Movement.MovementComplexity.Easy;
                }
            }
        }
    }
}

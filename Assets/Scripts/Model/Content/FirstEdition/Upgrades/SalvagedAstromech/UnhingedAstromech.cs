using System.Collections;
using System.Collections.Generic;
using Upgrade;
using Movement;

namespace UpgradesList.FirstEdition
{
    public class UnhingedAstromech : GenericUpgrade
    {
        public UnhingedAstromech() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Unhinged Astromech",
                UpgradeType.SalvagedAstromech,
                cost: 1,
                abilityType: typeof(Abilities.FirstEdition.UnhingedAstromechAbility)
            );
        }
    }
}

namespace Abilities.FirstEdition
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

        private void CheckAbility(Ship.GenericShip ship, ref ManeuverHolder movement)
        {
            if (movement.ColorComplexity != MovementComplexity.None)
            {
                if (movement.Speed == ManeuverSpeed.Speed3)
                {
                    movement.ColorComplexity = MovementComplexity.Easy;
                }
            }
        }

    }
}
using Upgrade;
using Ship;
using Movement;

namespace UpgradesList.FirstEdition
{
    public class R2Astromech : GenericUpgrade
    {
        public R2Astromech() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "R2 Astromech",
                UpgradeType.Astromech,
                cost: 1,
                abilityType: typeof(Abilities.FirstEdition.R2AstromechAbility)
            );
        }
    }
}

namespace Abilities.FirstEdition
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

        private void CheckR2AstromechAbility(GenericShip ship, ref ManeuverHolder movement)
        {
            if (movement.ColorComplexity != MovementComplexity.None)
            {
                if ((movement.Speed == ManeuverSpeed.Speed1) || (movement.Speed == ManeuverSpeed.Speed2))
                {
                    movement.ColorComplexity = MovementComplexity.Easy;
                }
            }
        }
    }
}
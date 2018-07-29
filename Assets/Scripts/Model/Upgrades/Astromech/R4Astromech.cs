using System.Collections;
using System.Collections.Generic;
using Upgrade;
using RuleSets;
using Ship;
using Movement;

namespace UpgradesList
{

    public class R4Astromech : GenericUpgrade
    {
        public R4Astromech() : base()
        {
            Types.Add(UpgradeType.Astromech);
            Name = "R4 Astromech";
            Cost = 2;

            UpgradeRuleType = typeof(SecondEdition);

            UpgradeAbilities.Add(new Abilities.SecondEdition.R4AstromechAbilitySE());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.ShipBaseSize == BaseSize.Small;
        }
    }

}

namespace Abilities
{
    namespace SecondEdition
    {
        public class R4AstromechAbilitySE : GenericAbility
        {
            public override void ActivateAbility()
            {
                HostShip.AfterGetManeuverColorDecreaseComplexity += ApplyAbility;
            }

            public override void DeactivateAbility()
            {
                HostShip.AfterGetManeuverColorDecreaseComplexity -= ApplyAbility;
            }

            private void ApplyAbility(GenericShip ship, ref MovementStruct movement)
            {
                if (movement.Speed == ManeuverSpeed.Speed1 || movement.Speed == ManeuverSpeed.Speed2)
                {
                    if (movement.Bearing == ManeuverBearing.Straight || movement.Bearing == ManeuverBearing.Bank || movement.Bearing == ManeuverBearing.Turn)
                    {
                        movement.ColorComplexity = ReduceComplexity(movement.ColorComplexity);
                    }
                }
            }

            private MovementComplexity ReduceComplexity(MovementComplexity complexity)
            {
                switch (complexity)
                {
                    case MovementComplexity.Normal:
                        complexity = MovementComplexity.Easy;
                        break;
                    case MovementComplexity.Complex:
                        complexity = MovementComplexity.Normal;
                        break;
                }

                return complexity;
            }
        }
    }
}

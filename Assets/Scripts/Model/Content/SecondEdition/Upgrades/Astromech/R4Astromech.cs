﻿using Upgrade;
using Ship;
using Movement;

namespace UpgradesList.SecondEdition
{
    public class R4Astromech : GenericUpgrade
    {
        public R4Astromech() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "R4 Astromech",
                UpgradeType.Astromech,
                cost: 2,
                abilityType: typeof(Abilities.SecondEdition.R4AstromechAbility),
                restriction: new BaseSizeRestriction(BaseSize.Small),
                seImageNumber: 55
            );
        }
    }
}

namespace Abilities.SecondEdition
{
    public class R4AstromechAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGetManeuverColorDecreaseComplexity += ApplyAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGetManeuverColorDecreaseComplexity -= ApplyAbility;
        }

        private void ApplyAbility(GenericShip ship, ref ManeuverHolder movement)
        {
            if (movement.Speed == ManeuverSpeed.Speed1 || movement.Speed == ManeuverSpeed.Speed2)
            {
                if (movement.Bearing == ManeuverBearing.Straight || movement.Bearing == ManeuverBearing.Bank || movement.Bearing == ManeuverBearing.Turn)
                {
                    movement.ColorComplexity = GenericMovement.ReduceComplexity(movement.ColorComplexity);
                    // Update revealed dial in UI
                    Roster.UpdateAssignedManeuverDial(HostShip, HostShip.AssignedManeuver);
                    Messages.ShowInfoToHuman("R4 Astromech: Difficulty of maneuvers is reduced");
                }
            }
        }
    }
}
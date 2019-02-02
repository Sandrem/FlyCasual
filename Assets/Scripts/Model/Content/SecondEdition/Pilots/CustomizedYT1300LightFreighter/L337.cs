using Movement;
using Ship;
using System.Collections;
using System.Collections.Generic;

namespace Ship
{
    namespace SecondEdition.CustomizedYT1300LightFreighter
    {
        public class L337 : CustomizedYT1300LightFreighter
        {
            public L337() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "L3-37",
                    2,
                    47,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.L337Ability),
                    seImageNumber: 224
                );

                ShipInfo.ActionIcons.SwitchToDroidActions();
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

        private void CheckAbility(GenericShip ship, ref ManeuverHolder movement)
        {
            if (HostShip.State.ShieldsCurrent == 0 && movement.Bearing == ManeuverBearing.Bank)
            {
                movement.ColorComplexity = GenericMovement.ReduceComplexity(movement.ColorComplexity);
            }
        }
    }
}

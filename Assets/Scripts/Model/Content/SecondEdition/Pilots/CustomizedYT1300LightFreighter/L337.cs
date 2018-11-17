using Actions;
using ActionsList;
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
                    limited: 1,
                    abilityType: typeof(Abilities.SecondEdition.L337Ability)
                );

                ShipInfo.ActionIcons.Actions.RemoveAll(a => a.ActionType == typeof(FocusAction));
                ShipInfo.ActionIcons.Actions.Add(new ActionInfo(typeof(CalculateAction)));

                SEImageNumber = 224;
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

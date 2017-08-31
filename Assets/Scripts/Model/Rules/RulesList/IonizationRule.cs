using UnityEngine;
using Ship;
using Tokens;
using Movement;

namespace RulesList
{
    public class IonizationRule
    {

        public IonizationRule()
        {
           GenericShip.OnTokenIsAssignedGlobal += CheckIonization;
        }

        private void CheckIonization(GenericShip ship, System.Type tokenType)
        {
            if (tokenType == typeof(IonToken))
            {
                if (ship.GetToken(typeof(IonToken)).Count == 1)
                {
                    Messages.ShowError("Ship is ionized!");
                    DoIonized(ship);
                }
            }
        }

        private void DoIonized(GenericShip ship)
        {
            ship.OnManeuverIsReadyToBeRevealed += AssignWhiteForwardOneManeuver;
            ship.OnMovementExecuted += RemoveIonization;
            ship.ToggleIonized(true);
        }

        private void AssignWhiteForwardOneManeuver(GenericShip ship)
        {
            ship.AssignedManeuver = new StraightMovement(1, ManeuverDirection.Forward, ManeuverBearing.Straight, ManeuverColor.White);

            ship.OnManeuverIsReadyToBeRevealed -= AssignWhiteForwardOneManeuver;
        }

        private void RemoveIonization(GenericShip ship)
        {
            ship.RemoveToken(typeof(IonToken), '*', true);
            ship.ToggleIonized(false);
            Messages.ShowInfo("Ship isn't ionized anymore");

            ship.OnMovementExecuted -= RemoveIonization;
        }

    }
}

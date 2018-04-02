using UnityEngine;
using Ship;
using Tokens;
using Movement;
using System.Linq;

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
                int ionTokensCount = ship.Tokens.GetAllTokens().Count(n => n is IonToken);

                if (ionTokensCount == 1 && ship.ShipBaseSize == BaseSize.Small )
                {
                    Messages.ShowError("Ship is ionized!");
                    DoIonized(ship);
                }
                if (ionTokensCount == 2 && ship.ShipBaseSize == BaseSize.Large)
                {
                    Messages.ShowError("Ship is ionized!");
                    DoIonized(ship);
                }
            }
        }

        private void DoIonized(GenericShip ship)
        {
            ship.OnManeuverIsReadyToBeRevealed += AssignWhiteForwardOneManeuver;
            ship.OnMovementExecuted += RegisterRemoveIonization;
            ship.ToggleIonized(true);
        }

        private void AssignWhiteForwardOneManeuver(GenericShip ship)
        {
            GenericMovement ionizedMovement = new StraightMovement(1, ManeuverDirection.Forward, ManeuverBearing.Straight, ManeuverColor.White) { IsRealMovement = false };
            ship.SetAssignedManeuver(ionizedMovement);

            ship.OnManeuverIsReadyToBeRevealed -= AssignWhiteForwardOneManeuver;
        }

        private void RegisterRemoveIonization(GenericShip ship)
        {
            if (Board.BoardManager.IsOffTheBoard(ship)) return;

            ship.OnMovementExecuted -= RegisterRemoveIonization;

            Triggers.RegisterTrigger(new Trigger
            {
                Name = "Remove ionization",
                TriggerType = TriggerTypes.OnShipMovementExecuted,
                TriggerOwner = ship.Owner.PlayerNo,
                EventHandler = RemoveIonization,
                Sender = ship
            });
        }

        private void RemoveIonization(object sender, System.EventArgs e)
        {
            Messages.ShowInfo("Ship isn't ionized anymore");

            GenericShip ship = sender as GenericShip;
            ship.ToggleIonized(false);

            ship.Tokens.RemoveAllTokensByType(
                typeof(IonToken),
                Triggers.FinishTrigger
            );
        }

    }
}

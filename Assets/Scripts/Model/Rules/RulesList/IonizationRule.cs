using UnityEngine;
using Ship;
using Tokens;
using Movement;
using System.Linq;
using RuleSets;

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

                if (ionTokensCount == RuleSet.Instance.NegativeTokensToAffectShip[ship.ShipBaseSize])
                {
                    Messages.ShowError("Ship is ionized!");
                    DoIonized(ship);
                }
            }
        }

        private void DoIonized(GenericShip ship)
        {
            ship.OnManeuverIsReadyToBeRevealed += AssignIonizationManeuver;
            ship.OnMovementExecuted += RegisterRemoveIonization;
            ship.ToggleIonized(true);
        }

        private void AssignIonizationManeuver(GenericShip ship)
        {
            GenericMovement ionizedMovement = new StraightMovement(1, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Normal) {
                IsRevealDial = false, IsIonManeuver = true
            };
            ship.SetAssignedManeuver(ionizedMovement);

            ship.OnManeuverIsReadyToBeRevealed -= AssignIonizationManeuver;
        }

        private void RegisterRemoveIonization(GenericShip ship)
        {
            if (BoardTools.Board.IsOffTheBoard(ship)) return;

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

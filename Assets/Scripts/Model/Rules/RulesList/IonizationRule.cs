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
        static bool RuleIsInitialized = false;

        public IonizationRule()
        {
            if (!RuleIsInitialized)
            {
                GenericShip.OnNoManeuverWasRevealedGlobal += SetIonManeuver;
                RuleIsInitialized = true;
            }
        }

        public static void SetIonManeuver(GenericShip ship)
        {
            if (IsIonized(ship))
            {
                AssignIonizationManeuver(ship);
                ship.OnMovementExecuted += RegisterRemoveIonization;
                RuleSet.Instance.WhenIonized(ship);
            }
        }

        private static void AssignIonizationManeuver(GenericShip ship)
        {
            GenericMovement ionizedMovement = new StraightMovement(1, ManeuverDirection.Forward, ManeuverBearing.Straight, RuleSet.Instance.IonManeuverComplexity) {
                IsRevealDial = false, IsIonManeuver = true
            };
            ship.SetAssignedManeuver(ionizedMovement);
        }

        private static void RegisterRemoveIonization(GenericShip ship)
        {
            if (BoardTools.Board.IsOffTheBoard(ship)) return;

            ship.OnMovementExecuted -= RegisterRemoveIonization;

            Triggers.RegisterTrigger(new Trigger
            {
                Name = "Remove ionization",
                TriggerType = TriggerTypes.OnMovementExecuted,
                TriggerOwner = ship.Owner.PlayerNo,
                EventHandler = RemoveIonization,
                Sender = ship
            });
        }

        private static void RemoveIonization(object sender, System.EventArgs e)
        {
            Messages.ShowInfo("Ship isn't ionized anymore");

            GenericShip ship = sender as GenericShip;
            ship.ToggleIonized(false);

            ship.Tokens.RemoveAllTokensByType(
                typeof(IonToken),
                Triggers.FinishTrigger
            );
        }

        public static bool IsIonized(GenericShip ship)
        {
            int ionTokensCount = ship.Tokens.GetAllTokens().Count(n => n is IonToken);
            return (ionTokensCount >= RuleSet.Instance.NegativeTokensToAffectShip[ship.ShipBaseSize]);
        }

    }
}

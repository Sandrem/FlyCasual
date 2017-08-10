using System.Collections.Generic;
using UnityEngine;

namespace RulesList
{
    public class CollisionRules
    {
        private GameManagerScript Game;

        public CollisionRules(GameManagerScript game)
        {
            Game = game;
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            TargetIsLegalForShotRule.OnCheckTargetIsLegal += CanPerformAttack;
            Phases.BeforeActionSubPhaseStart += CheckSkipPerformAction;
        }

        public void CheckSkipPerformAction()
        {
            if (Selection.ThisShip.IsBumped)
            {
                Messages.ShowErrorToHuman("Collided into ship - action subphase is skipped");
                Selection.ThisShip.IsSkipsActionSubPhase = true;
            }
        }

        public void ClearBumps(Ship.GenericShip ship)
        {
            foreach (var bumpedShip in ship.ShipsBumped)
            {
                if (bumpedShip.ShipsBumped.Contains(ship))
                {
                    bumpedShip.ShipsBumped.Remove(ship);
                }
            }
            ship.ShipsBumped = new List<Ship.GenericShip>();
        }

        public void CanPerformAttack(ref bool result, Ship.GenericShip attacker, Ship.GenericShip defender)
        {
            if ((attacker.IsBumped) && (attacker.ShipsBumped.Contains(defender)) && (defender.ShipsBumped.Contains(attacker)))
            {
                Messages.ShowErrorToHuman("Cannot attack ship that you are touching");
                result = false;
            }
        }

    }
}

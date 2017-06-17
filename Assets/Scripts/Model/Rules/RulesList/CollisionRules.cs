
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
            Actions.OnCheckTargetIsLegal += CanPerformAttack;
            Phases.OnActionSubPhaseStart += CheckSkipPerformAction;
        }

        public void CheckSkipPerformAction()
        {
            if (Selection.ThisShip.IsBumped)
            {
                Game.UI.ShowError("Collided into ship - action subphase is skipped");
                Selection.ThisShip.IsSkipsActionSubPhase = true;
                Selection.ThisShip.IsBumped = false;
            }
        }

        public void ClearCollision(Ship.GenericShip ship)
        {
            ship.LastShipCollision = null;
        }

        public void AssignCollision(Ship.GenericShip ship)
        {
            // BUG: Sometimes NullReferenceException: Object reference not set to an instance of an object
            (Selection.ThisShip.LastShipCollision).LastShipCollision = Selection.ThisShip;
            ship.IsBumped = true;
        }

        public void CanPerformAttack(ref bool result, Ship.GenericShip attacker, Ship.GenericShip defender)
        {
            if ((attacker.LastShipCollision != null) && (attacker.LastShipCollision == defender) && (defender.LastShipCollision == attacker))
            {
                Game.UI.ShowError("Cannot attack ship that you are touching");
                result = false;
            }
        }

    }
}

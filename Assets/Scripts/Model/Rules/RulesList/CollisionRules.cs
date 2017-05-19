
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
            Actions.OnCheckCanPerformAttack += CanPerformAttack;
        }

        public void ClearCollision(Ship.GenericShip ship)
        {
            ship.LastShipCollision = null;
        }

        public void AssignCollision(Ship.GenericShip ship)
        {
            (Game.Selection.ThisShip.LastShipCollision).LastShipCollision = Game.Selection.ThisShip;
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

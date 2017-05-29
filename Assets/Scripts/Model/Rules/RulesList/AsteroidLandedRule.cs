using UnityEngine;

namespace RulesList
{
    public class AsteroidLandedRule
    {
        private GameManagerScript Game;

        public AsteroidLandedRule(GameManagerScript game)
        {
            Game = game;
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            Actions.OnCheckTargetIsLegal += CanPerformAttack;
        }

        public void CanPerformAttack(ref bool result, Ship.GenericShip attacker, Ship.GenericShip defender)
        {
            foreach (var obstacle in attacker.ObstaclesLanded)
            {
                if (obstacle.tag == "Asteroid")
                {
                    Game.UI.ShowError("Landed on asteroid - cannot attack");
                    result = false;
                    break;
                }
            }
        }

    }
}

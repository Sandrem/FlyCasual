using UnityEngine;

namespace RulesList
{
    public class AsteroidObstructionRule
    {
        private GameManagerScript Game;

        public AsteroidObstructionRule(GameManagerScript game)
        {
            Game = game;
        }

        public void CheckDefenceDistanceBonus(ref int result)
        {
            int obstructions = Board.BoardManager.FiringLineCollisions.Count;
            if (obstructions > 0)
            {
                string notification = "Obstacle obstruction bonus: +" + obstructions + " defence dice";
                if (obstructions > 1)
                {
                    notification = notification + "s";
                }
                Game.UI.ShowInfo(notification);
                result = result + obstructions;
            }
        }

    }
}

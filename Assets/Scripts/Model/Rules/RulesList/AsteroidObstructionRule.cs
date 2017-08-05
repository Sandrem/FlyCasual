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
            if (Combat.IsObstructed)
            {
                string notification = "Obstruction bonus: +1 defence dice";
                Game.UI.ShowInfo(notification);
                result++;
            }
        }

    }
}

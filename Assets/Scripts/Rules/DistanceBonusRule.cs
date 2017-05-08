
using UnityEngine;

namespace Rules
{
    public class DistanceBonusRule
    {
        private GameManagerScript Game;

        public DistanceBonusRule(GameManagerScript game)
        {
            Game = game;
        }

        public void CheckAttackDistanceBonus(ref int result)
        {
            if (Game.Actions.GetRange(Game.Combat.Attacker, Game.Combat.Defender) == 1)
            {
                Game.UI.ShowInfo("Distance bonus: +1 attack die");
                result++;
            }
        }

        public void CheckDefenceDistanceBonus(ref int result)
        {
            if (Game.Actions.GetRange(Game.Combat.Attacker, Game.Combat.Defender) == 3)
            {
                Game.UI.ShowInfo("Distance bonus: +1 defence die");
                result++;
            }
        }

    }
}

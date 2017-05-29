
using UnityEngine;

namespace RulesList
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
            if (Actions.GetRange(Combat.Attacker, Combat.Defender) == 1)
            {
                Game.UI.ShowInfo("Distance bonus: +1 attack dice");
                result++;
            }
        }

        public void CheckDefenceDistanceBonus(ref int result)
        {
            if (Actions.GetRange(Combat.Attacker, Combat.Defender) == 3)
            {
                Game.UI.ShowInfo("Distance bonus: +1 defence dice");
                result++;
            }
        }

    }
}


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
            Board.ShipShotDistanceInformation shotInfo = new Board.ShipShotDistanceInformation(Combat.Attacker, Combat.Defender);
            if (shotInfo.Range == 1)
            {
                Game.UI.ShowInfo("Distance bonus: +1 attack dice");
                result++;
            }
        }

        public void CheckDefenceDistanceBonus(ref int result)
        {
            Board.ShipShotDistanceInformation shotInfo = new Board.ShipShotDistanceInformation(Combat.Attacker, Combat.Defender);
            if (shotInfo.Range == 3)
            {
                Game.UI.ShowInfo("Distance bonus: +1 defence dice");
                result++;
            }
        }

    }
}


using UnityEngine;

namespace RulesList
{
    public class DistanceBonusRule
    {

        public void CheckAttackDistanceBonus(ref int result)
        {
            Board.ShipShotDistanceInformation shotInfo = new Board.ShipShotDistanceInformation(Combat.Attacker, Combat.Defender);
            if (shotInfo.Range == 1)
            {
                Messages.ShowInfo("Distance bonus: +1 attack dice");
                result++;
            }
        }

        public void CheckDefenceDistanceBonus(ref int result)
        {
            Board.ShipShotDistanceInformation shotInfo = new Board.ShipShotDistanceInformation(Combat.Attacker, Combat.Defender);
            if (shotInfo.Range == 3)
            {
                Messages.ShowInfo("Distance bonus: +1 defence dice");
                result++;
            }
        }

    }
}

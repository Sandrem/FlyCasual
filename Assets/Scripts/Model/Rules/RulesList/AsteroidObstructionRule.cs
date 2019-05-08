using UnityEngine;

namespace RulesList
{
    public class AsteroidObstructionRule
    {

        public void CheckDefenceObstructionBonus(ref int result)
        {
            if (Combat.ShotInfo.IsObstructedByAsteroid && !Combat.Attacker.IsIgnoreObstacleObstructionWhenAttacking)
            {
                Messages.ShowInfo("The attack is obstructed, giving the defender +1 defense die");
                result++;
            }
        }

    }
}

using UnityEngine;

namespace RulesList
{
    public class AsteroidObstructionRule
    {

        public void CheckDefenceObstructionBonus(ref int result)
        {
            if (Combat.ShotInfo.IsObstructedByAsteroid && !Combat.Attacker.IsIgnoreObstacleObstructionWhenAttacking)
            {
                Messages.ShowInfo("Obstruction bonus: +1 defence die");
                result++;
            }
        }

    }
}

using UnityEngine;

namespace RulesList
{
    public class AsteroidObstructionRule
    {

        public void CheckDefenceObstructionBonus(ref int result)
        {
            if (Combat.ShotInfo.IsObstructedByAsteroid)
            {
                Messages.ShowInfo("Obstruction bonus: +1 defence dice");
                result++;
            }
        }

    }
}

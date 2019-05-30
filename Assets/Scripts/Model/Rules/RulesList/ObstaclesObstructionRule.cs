using Obstacles;
using Ship;
using UnityEngine;

namespace RulesList
{
    public class AsteroidObstructionRule
    {
        static bool RuleIsInitialized = false;

        public AsteroidObstructionRule()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            if (!RuleIsInitialized)
            {
                GenericShip.AfterGotNumberOfDefenceDiceGlobal += CheckDefenceObstructionBonus;
                RuleIsInitialized = true;
            }
        }

        public void CheckDefenceObstructionBonus(ref int result)
        {
            foreach (GenericShip ship in Combat.ShotInfo.ObstructedByShips)
            {
                ship.CallShotObstructedByMe(Combat.Attacker, ref result);
            }

            if (Combat.ShotInfo.IsObstructedByObstacle && !Combat.Attacker.IsIgnoreObstacleObstructionWhenAttacking)
            {
                Messages.ShowInfo("The attack is obstructed, giving the defender +1 defense die");
                result++;

                foreach (GenericObstacle obstacle in Combat.ShotInfo.ObstructedByObstacles)
                {
                    obstacle.OnShotObstructedExtra(Combat.Attacker, Combat.Defender);
                }
            }
        }

    }
}

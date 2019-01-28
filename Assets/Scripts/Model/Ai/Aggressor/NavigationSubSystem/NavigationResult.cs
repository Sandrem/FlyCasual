using Movement;
using Ship;
using System.Collections.Generic;
using System.Linq;

namespace AI.Aggressor
{
    public class NavigationResult
    {
        public bool isOffTheBoard;
        public bool isLandedOnObstacle;

        public int enemiesInShotRange;

        public int obstaclesHit;
        public int minesHit;

        public float distanceToNearestEnemy;
        public float distanceToNearestEnemyInShotRange;

        public bool isOffTheBoardNextTurn;
        public bool isHitAsteroidNextTurn;

        public bool isBumped;

        public MovementComplexity movementComplexity;

        public int Priority { get; private set; }

        public ShipPositionInfo FinalPositionInfo { get; set; }

        public void CalculatePriority()
        {
            if (isOffTheBoard)
            {
                Priority = int.MinValue;
                return;
            }

            if (isLandedOnObstacle) Priority -= 10000;

            if (isOffTheBoardNextTurn) Priority -= 20000;

            // TODO: Koigogran Turn ignores rotation
            Priority += enemiesInShotRange * 1000;

            Priority -= obstaclesHit * 2000;
            Priority -= minesHit * 2000;

            if (isHitAsteroidNextTurn) Priority -= 1000;

            if (isBumped) Priority -= 1000;

            switch (movementComplexity)
            {
                case MovementComplexity.Easy:
                    if (Selection.ThisShip.IsStressed) Priority += 500;
                    break;
                case MovementComplexity.Complex:
                    if (Selection.ThisShip.IsStressed)
                    {
                        Priority = int.MinValue;
                    }
                    else
                    {
                        Priority -= 500;
                    }
                    break;
                default:
                    break;
            }

            //distance is 0..10
            Priority += (10 - (int)distanceToNearestEnemy) * 10;
        }
    }
}

using Movement;
using Ship;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AI.Aggressor
{
    public class NavigationResult
    {
        public bool isOffTheBoard;
        public bool isLandedOnObstacle;

        public int enemiesInShotRange;
        public int enemiesTargetingThisShip;

        public int obstaclesHit;
        public int minesHit;

        public float distanceToNearestEnemy;
        public float distanceToNearestEnemyInShotRange;

        public bool isOffTheBoardNextTurn;
        public bool isHitAsteroidNextTurn;

        public bool isBumped;

        public GenericMovement movement;

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

            // Base our priority off of how many enemies can shoot us versus how many we can shoot.
            Priority += enemiesInShotRange * 1000;
            // Allow up to two enemies targeting us to equal one enemy in our attack range.
            // Priority -= enemiesTargetingThisShip * 500;

            Priority -= obstaclesHit * 2000;
            Priority -= minesHit * 2000;

            if (isHitAsteroidNextTurn) Priority -= 1000;

            if (isBumped) Priority -= 1000;

            if (Selection.ThisShip.Damage.HasCrit(typeof(DamageDeckCardSE.LooseStabilizer)) && movement.Bearing != ManeuverBearing.Straight)
            {
                if (Selection.ThisShip.State.HullCurrent + Selection.ThisShip.State.ShieldsCurrent == 1)
                {
                    Priority -= 20000;
                }
                else
                {
                    Priority -= 1000;
                }
            }

            switch (movement.ColorComplexity)
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
                case MovementComplexity.None: // Impossible maneuvers
                    Priority = int.MinValue;
                    break;
                default:
                    break;
            }

            //distance is 0..10
            Priority += (10 - (int)distanceToNearestEnemy) * 10;
        }
    }
}

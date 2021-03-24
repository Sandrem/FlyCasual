using BoardTools;
using Obstacles;

namespace Abilities
{
    public class AsteroidRangeCondition : Condition
    {
        private int MinRange;
        private int MaxRange;

        public AsteroidRangeCondition(int minRange = 0, int maxRange = 3)
        {
            MinRange = minRange;
            MaxRange = maxRange;
        }

        public override bool Passed(ConditionArgs args)
        {
            if (args.ShipToCheck == null)
            {
                Messages.ShowError("Ability Condition Error: ship is not set");
                return false;
            }

            foreach (GenericObstacle obstacle in Obstacles.ObstaclesManager.GetPlacedObstacles())
            {
                if (obstacle is Asteroid)
                {
                    ShipObstacleDistance distInfo = new ShipObstacleDistance(args.ShipToCheck, obstacle);
                    if (distInfo.Range >= MinRange && distInfo.Range <= MaxRange) return true;
                }
            }
            return false;
        }
    }
}

using Ship;
using System.Linq;
using Obstacles;

namespace BoardTools
{
    public class ShipObstacleDistance
    {
        public int Range { get; private set; }

        public ShipObstacleDistance(GenericShip ship, GenericObstacle obstacle)
        {
            Range = 1;
        }
    }
}

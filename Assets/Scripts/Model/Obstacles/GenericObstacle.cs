using UnityEngine;

namespace Obstacles
{
    public class GenericObstacle: IDamageSource
    {
        public string Name { get; protected set; }
        public bool IsPlaced { get; set; }
        public GameObject ObstacleGO { get; set; }

        public GenericObstacle(GameObject obstacleGO)
        {
            ObstacleGO = obstacleGO;
        }
    }
}

using Bombs;
using System.Linq;
using Obstacles;
using UnityEngine;
using Upgrade;

namespace BoardTools
{
    public class BombObstacleDistance
    {
        public int Range { get; private set; }
        public GenericBomb Bomb { get; private set; }
        public GenericObstacle Obstacle { get; private set; }

        public BombObstacleDistance(GenericBomb bomb, GenericObstacle obstacle)
        {
            Bomb = bomb;
            Obstacle = obstacle;

            if (!CheckDistanceSimple()) CheckDistanceAdvanced();
        }

        private bool CheckDistanceSimple()
        {
            foreach (Vector3 bombPoint in BombsManager.GetBombPoints(Bomb))
            {
                float distanceBetween = Vector3.Distance(bombPoint, Obstacle.ObstacleGO.transform.position);

                Range = Mathf.Max(1, Mathf.CeilToInt(distanceBetween / Board.DISTANCE_INTO_RANGE));

                if (Range == 1) return true;
                if (Range > 4) return true;
            }

            return false;
        }

        private void CheckDistanceAdvanced()
        {
            float minDistance = float.MaxValue;

            foreach (Vector3 bombPoint in BombsManager.GetBombPoints(Bomb))
            {
                MeshCollider obstacleCollider = Obstacle.ObstacleGO.GetComponentInChildren<MeshCollider>();
                Vector3 obstacleColliderPoint = obstacleCollider.ClosestPoint(bombPoint);

                float distanceBetweenEdges = Vector3.Distance(bombPoint, obstacleColliderPoint);
                if (distanceBetweenEdges < minDistance) minDistance = distanceBetweenEdges;
            }
            
            Range = Mathf.Max(1, Mathf.CeilToInt(minDistance / Board.DISTANCE_INTO_RANGE));
        }
    }
}

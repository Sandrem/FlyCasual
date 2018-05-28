using Ship;
using System.Linq;
using Obstacles;
using UnityEngine;

namespace BoardTools
{
    public class ShipObstacleDistance
    {
        public int Range { get; private set; }
        public GenericShip Ship { get; private set; }
        public GenericObstacle Obstacle { get; private set; }

        public ShipObstacleDistance(GenericShip ship, GenericObstacle obstacle)
        {
            Ship = ship;
            Obstacle = obstacle;

            if (!CheckDistanceSimple()) CheckDistanceAdvanced();
        }

        private bool CheckDistanceSimple()
        {
            float distanceBetweenCenters = Vector3.Distance(Ship.GetPosition(), Obstacle.ObstacleGO.transform.position);

            Range = Mathf.Max(1, Mathf.CeilToInt(distanceBetweenCenters / Board.DISTANCE_INTO_RANGE));

            if (Range == 1) return true;
            if (Range > 4) return true;

            return false;
        }

        private void CheckDistanceAdvanced()
        {
            MeshCollider obstacleCollider = Obstacle.ObstacleGO.GetComponentInChildren<MeshCollider>();
            Vector3 obstacleColliderPoint = obstacleCollider.ClosestPoint(Ship.GetPosition());

            MeshCollider shipCollider = Ship.GetCollider();
            Vector3 shipColliderPoint = shipCollider.ClosestPoint(obstacleColliderPoint);

            float distanceBetweenEdges = Vector3.Distance(shipColliderPoint, obstacleColliderPoint);
            Range = Mathf.Max(1, Mathf.CeilToInt(distanceBetweenEdges / Board.DISTANCE_INTO_RANGE));
        }
    }
}

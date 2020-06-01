using Ship;
using System.Linq;
using Obstacles;
using UnityEngine;

namespace BoardTools
{
    public class ShipObstacleDistance
    {
        public int Range { get; private set; }
        public float DistanceReal { get; private set; }
        public GenericShip Ship { get; private set; }
        public GenericObstacle Obstacle { get; private set; }
        public Vector3 NearestPointShip { get; private set; }
        public Vector3 NearestPointObstacle { get; private set; }

        public ShipObstacleDistance(GenericShip ship, GenericObstacle obstacle)
        {
            CollisionDetectionQuality currentQuality = ObstaclesManager.CollisionDetectionQuality;
            ObstaclesManager.SetObstaclesCollisionDetectionQuality(CollisionDetectionQuality.Low);
            Ship = ship;
            Obstacle = obstacle;

            if (!CheckDistanceSimple()) CheckDistanceAdvanced();
            ObstaclesManager.SetObstaclesCollisionDetectionQuality(currentQuality);
        }

        private bool CheckDistanceSimple()
        {
            NearestPointShip = Ship.GetPosition();
            NearestPointObstacle = Obstacle.ObstacleGO.transform.position;
            DistanceReal = Vector3.Distance(NearestPointShip, NearestPointObstacle);

            Range = Mathf.Max(1, Mathf.CeilToInt(DistanceReal / Board.DISTANCE_INTO_RANGE));

            if (Range == 1) return true;
            if (Range > 4) return true;

            return false;
        }

        private void CheckDistanceAdvanced()
        {
            MeshCollider obstacleCollider = Obstacle.ObstacleGO.GetComponentInChildren<MeshCollider>();
            NearestPointObstacle = obstacleCollider.ClosestPoint(Ship.GetPosition());

            MeshCollider shipCollider = Ship.GetCollider();
            NearestPointShip = shipCollider.ClosestPoint(NearestPointObstacle);

            DistanceReal = Vector3.Distance(NearestPointShip, NearestPointObstacle);
            Range = Mathf.Max(1, Mathf.CeilToInt(DistanceReal / Board.DISTANCE_INTO_RANGE));
        }
    }
}

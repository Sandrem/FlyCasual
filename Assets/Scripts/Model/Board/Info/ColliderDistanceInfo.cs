using Obstacles;
using UnityEngine;

namespace BoardTools
{
    public class ColliderDistanceInfo
    {
        public int Range { get; private set; }
        public float DistanceReal { get; private set; }
        public IBoardObject ObjectA { get; private set; }
        public IBoardObject ObjectB { get; private set; }
        public Vector3 NearestPointObjectA { get; private set; }
        public Vector3 NearestPointObjectB { get; private set; }

        public ColliderDistanceInfo(IBoardObject objectA, IBoardObject objectB)
        {
            CollisionDetectionQuality currentQuality = ObstaclesManager.CollisionDetectionQuality;
            ObstaclesManager.SetObstaclesCollisionDetectionQuality(CollisionDetectionQuality.Low);

            ObjectA = objectA;
            ObjectB = objectB;

            CheckDistance();

            ObstaclesManager.SetObstaclesCollisionDetectionQuality(currentQuality);
        }

        private void CheckDistance()
        {
            NearestPointObjectA = ObjectA.Collider.ClosestPoint(ObjectB.Collider.transform.position);

            //Temporary
            /*GameObject point1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            point1.transform.position = NearestPointObjectA;
            point1.transform.localScale = new Vector3(0.01f, 1, 0.01f);*/

            NearestPointObjectB = ObjectB.Collider.ClosestPoint(ObjectA.Collider.transform.position);

            //Temporary
            /*GameObject point2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            point2.transform.position = NearestPointObjectB;
            point2.transform.localScale = new Vector3(0.01f, 1, 0.01f);*/

            DistanceReal = Vector3.Distance(NearestPointObjectA, NearestPointObjectB);
            Range = Board.DistanceToRange(DistanceReal);
        }
    }
}
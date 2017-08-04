using Ship;
using UnityEngine;

namespace Board
{

    public class ShipDistanceInformation
    {
        //TODO: REWORK
        private static readonly float DISTANCE_1 = 3.28f / 3f;

        public Vector3 ThisShipNearestPoint { get; protected set; }
        public Vector3 AnotherShipNearestPoint { get; protected set; }
        public float Distance{ get; protected set; }
        public Vector3 Vector
        {
            get { return AnotherShipNearestPoint - ThisShipNearestPoint; }
        }
        public int Range
        {
            get { return Mathf.CeilToInt(Distance / DISTANCE_1); }
        }

        public ShipDistanceInformation(GenericShip thisShip, GenericShip anotherShip)
        {
            GetClosestPoints(thisShip, anotherShip);
        }

        protected virtual void GetClosestPoints(GenericShip thisShip, GenericShip anotherShip)
        {
            Distance = float.MaxValue;

            foreach (var objThis in thisShip.GetStandEdgePoints())
            {
                foreach (var objAnother in anotherShip.GetStandEdgePoints())
                {
                    float distance = Vector3.Distance(objThis.Value, objAnother.Value);
                    if (distance < Distance)
                    {
                        Distance = distance;
                        ThisShipNearestPoint = objThis.Value;
                        AnotherShipNearestPoint = objAnother.Value;
                    }
                }
            }
        }
    }

}

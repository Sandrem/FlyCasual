using Ship;
using UnityEngine;

namespace Board
{

    public class ShipDistanceInformation
    {
        protected GenericShip thisShip;
        protected GenericShip anotherShip;

        //TODO: REWORK
        protected static readonly float DISTANCE_1 = 3.28f / 3f;

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
            this.thisShip = thisShip;
            this.anotherShip = anotherShip;
            CalculateFields();
        }

        protected virtual void CalculateFields()
        {
            Distance = float.MaxValue;

            foreach (var objThis in thisShip.GetStandPoints())
            {
                foreach (var objAnother in anotherShip.GetStandPoints())
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

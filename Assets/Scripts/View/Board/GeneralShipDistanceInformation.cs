using Ship;
using UnityEngine;
using System.Collections.Generic;

namespace Board
{

    public class GeneralShipDistanceInformation
    {
        public GenericShip ThisShip { get; private set; }
        public GenericShip AnotherShip { get; private set; }

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

        public GeneralShipDistanceInformation(GenericShip thisShip, GenericShip anotherShip)
        {
            this.ThisShip = thisShip;
            this.AnotherShip = anotherShip;
        }

        protected virtual void CalculateFields()
        {
            CalculateFieldUsingPoints(ThisShip.GetStandPoints(), AnotherShip.GetStandPoints());
        }

        protected virtual void CalculateFieldUsingPoints(Dictionary<string, Vector3> pointsStart, Dictionary<string, Vector3> pointsEnd)
        {
            Distance = float.MaxValue;

            foreach (var objThis in pointsStart)
            {
                foreach (var objAnother in pointsEnd)
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

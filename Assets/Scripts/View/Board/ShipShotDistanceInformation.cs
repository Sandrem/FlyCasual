using Ship;
using UnityEngine;

namespace Board
{

    public class ShipShotDistanceInformation : ShipDistanceInformation
    {
        //public bool IsObstructed { get; protected set; }
        public bool InArc { get; private set; }

        public ShipShotDistanceInformation(GenericShip thisShip, GenericShip anotherShip) : base(thisShip, anotherShip) { }

        protected override void GetClosestPoints(GenericShip thisShip, GenericShip anotherShip)
        {
            Distance = float.MaxValue;
            Vector3 vectorFacing = thisShip.GetFrontFacing();
            InArc = false;

            foreach (var objThis in thisShip.GetStandFrontEdgePoins())
            {
                foreach (var objAnother in anotherShip.GetStandEdgePoints())
                {
                    float distance = Vector3.Distance(objThis.Value, objAnother.Value);
                    if (distance < Distance)
                    {
                        Vector3 vectorToTarget = objAnother.Value - objThis.Value;
                        float angle = Mathf.Abs(Vector3.Angle(vectorToTarget, vectorFacing));

                        if (angle <= 40)
                        {
                            InArc = true;

                            Distance = distance;
                            ThisShipNearestPoint = objThis.Value;
                            AnotherShipNearestPoint = objAnother.Value;
                        }
                    }
                }
            }
        }
    }

}

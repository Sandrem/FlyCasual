using Ship;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Board
{

    public class ShipShotOutOfArcDistanceInformation : ShipDistanceInformation
    {
        public ShipShotOutOfArcDistanceInformation(GenericShip thisShip, GenericShip anotherShip) : base(thisShip, anotherShip) { }

        protected override void CalculateFields()
        {
            CalculateFieldUsingPoints(ThisShip.GetStandFrontEdgePoins(), AnotherShip.GetStandPoints());
        }

    }

}

using Ship;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Board
{

    public class ShipShotOutOfArcDistanceInformation : GeneralShipDistanceInformation
    {

        public ShipShotOutOfArcDistanceInformation(GenericShip thisShip, GenericShip anotherShip) : base(thisShip, anotherShip)
        {
            CalculateFields();
        }

        protected override void CalculateFields()
        {
            CalculateFieldUsingPoints(ThisShip.ShipBase.GetStandFrontEdgePoins(), AnotherShip.ShipBase.GetStandPoints());
        }

    }

}

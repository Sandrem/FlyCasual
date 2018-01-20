using Ship;
using UnityEngine;
using System.Collections.Generic;

namespace Board
{

    public class ShipDistanceInformation : GeneralShipDistanceInformation
    {

        public ShipDistanceInformation(GenericShip thisShip, GenericShip anotherShip) : base(thisShip, anotherShip)
        {
            CalculateFields();
        }

    }
}

using Ship;
using UnityEngine;
using System.Collections.Generic;

namespace BoardTools
{

    public class ShipDistanceInformationOld : GeneralShipDistanceInformation
    {

        public ShipDistanceInformationOld(GenericShip thisShip, GenericShip anotherShip) : base(thisShip, anotherShip)
        {
            Calculate();
        }

    }
}

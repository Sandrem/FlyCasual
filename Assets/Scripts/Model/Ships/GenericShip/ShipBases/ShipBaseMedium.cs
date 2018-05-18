using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Ship
{
    public class ShipBaseMedium : GenericShipBase
    {

        public ShipBaseMedium(GenericShip host) : base(host)
        {
            Size = BaseSize.Medium;
            PrefabPath = "Prefabs/ShipModel/ShipBase/ShipBaseMedium";
            TemporaryPrefabPath = "Prefabs/ShipModel/ShipBase/TemporaryShipBaseMedium";

            HALF_OF_SHIPSTAND_SIZE = 0.75f;
            SHIPSTAND_SIZE = 1.5f;
            SHIPSTAND_SIZE_CM = 6f;
            HALF_OF_FIRINGARC_SIZE = 0.425f * SHIPSTAND_SIZE;

            CreateShipBase();
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Ship
{
    public class ShipBaseSmall : GenericShipBase
    {

        public ShipBaseSmall(GenericShip host) : base(host)
        {
            Size = BaseSize.Small;
            PrefabPath = "Prefabs/ShipModel/ShipBase/ShipBaseSmall";
            TemporaryPrefabPath = "Prefabs/ShipModel/ShipBase/TemporaryShipBaseSmall";

            HALF_OF_SHIPSTAND_SIZE = 0.5f;
            SHIPSTAND_SIZE = 1f;
            SHIPSTAND_SIZE_CM = 4f;
            HALF_OF_FIRINGARC_SIZE = 0.425f;

            CreateShipBase();
        }

    }
}

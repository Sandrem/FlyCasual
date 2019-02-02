﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Ship
{
    public class ShipBaseLarge : GenericShipBase
    {

        public ShipBaseLarge(GenericShip host) : base(host)
        {
            Size = BaseSize.Large;
            PrefabPath = "Prefabs/ShipModel/ShipBase/ShipBaseLarge";
            TemporaryPrefabPath = "Prefabs/ShipModel/ShipBase/TemporaryShipBaseLarge";

            HALF_OF_SHIPSTAND_SIZE = 1f;
            SHIPSTAND_SIZE = 2f;
            SHIPSTAND_SIZE_CM = 8f;

            HALF_OF_FIRINGARC_SIZE = 0.425f * SHIPSTAND_SIZE;

            CreateShipBase();
        }

    }
}

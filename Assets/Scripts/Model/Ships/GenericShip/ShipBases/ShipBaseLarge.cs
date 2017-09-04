using System;
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

            //TODO: calculate
            HALF_OF_FIRINGARC_SIZE = 0.425f;

            CreateShipBase();
        }

        protected override void CreateShipBase()
        {
            base.CreateShipBase();

            Host.GetShipAllPartsTransform().localPosition = Host.GetShipAllPartsTransform().localPosition + new Vector3(0, 0, -1);
        }

    }
}

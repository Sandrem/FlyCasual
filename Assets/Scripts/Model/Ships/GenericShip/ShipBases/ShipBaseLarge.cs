using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ship
{
    public class ShipBaseLarge : GenericShipBase
    {

        public ShipBaseLarge(GenericShip host) : base(host)
        {
            Size = BaseSize.Large;
            PrefabPath = "Prefabs/ShipModel/ShipBase/ShipBaseLarge";
            TemporaryPrefabPath = "Prefabs/ShipModel/ShipBase/TemporaryShipBaseLarge";

            CreateShipBase();
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ship
{
    public class ShipBaseSmall : GenericShipBase
    {

        public ShipBaseSmall(GenericShip host) : base(host)
        {
            Size = BaseSize.Small;
            PrefabPath = "Prefabs/ShipModel/ShipBase/ShipBaseSmall";
            TemporaryPrefabPath = "Prefabs/ShipModel/ShipBase/TemporaryShipBaseSmall";

            CreateShipBase();
        }

    }
}

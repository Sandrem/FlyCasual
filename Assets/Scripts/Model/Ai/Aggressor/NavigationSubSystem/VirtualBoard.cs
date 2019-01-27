using Ship;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AI.Aggressor
{
    public class VirtualShipInfo
    {
        public GenericShip Ship { get; private set; }
        public ShipPositionInfo RealPositionInfo { get; private set; }
        public ShipPositionInfo VirtualPositionInfo { get; set; }

        public VirtualShipInfo(GenericShip ship)
        {
            Ship = ship;
            RealPositionInfo = VirtualPositionInfo = new ShipPositionInfo(ship.GetPosition(), ship.GetAngles());
        }
    }

    public class VirtualBoard
    {
        public Dictionary<GenericShip, VirtualShipInfo> Ships;

        public VirtualBoard()
        {
            Ships = new Dictionary<GenericShip, VirtualShipInfo>();
            foreach (GenericShip ship in Roster.AllShips.Values)
            {
                Ships.Add(ship, new VirtualShipInfo(ship));
            }
        }

        public void SetVirtualPositionInfo(GenericShip ship, ShipPositionInfo virtualPositionInfo)
        {
            Ships[ship].VirtualPositionInfo = virtualPositionInfo;

            //Add debug mode
            ship.SetPositionInfo(virtualPositionInfo);
        }
    }
}

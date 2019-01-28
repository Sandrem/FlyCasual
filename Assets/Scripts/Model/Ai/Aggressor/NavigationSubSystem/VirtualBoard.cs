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
        public ShipPositionInfo VirtualPositionInfo { get; private set; }

        public VirtualShipInfo(GenericShip ship)
        {
            Ship = ship;
            RealPositionInfo = VirtualPositionInfo = new ShipPositionInfo(ship.GetPosition(), ship.GetAngles());
        }

        public void UpdateVirtualPositionInfo(ShipPositionInfo virtualPositionInfo)
        {
            VirtualPositionInfo = virtualPositionInfo;
        }

        public void UpdatePositionInfo(ShipPositionInfo positionInfo)
        {
            RealPositionInfo = VirtualPositionInfo = positionInfo;
        }
    }

    public class VirtualBoard
    {
        public Dictionary<GenericShip, VirtualShipInfo> Ships;
        public int Round;

        public VirtualBoard()
        {
            Update();
        }

        public void Update()
        {
            if (Round < Phases.RoundCounter)
            {
                Ships = new Dictionary<GenericShip, VirtualShipInfo>();
                foreach (GenericShip ship in Roster.AllShips.Values)
                {
                    Ships.Add(ship, new VirtualShipInfo(ship));
                }

                Round = Phases.RoundCounter;
            }
        }

        public void SetVirtualPositionInfo(GenericShip ship, ShipPositionInfo virtualPositionInfo)
        {
            Ships[ship].UpdateVirtualPositionInfo(virtualPositionInfo);
        }

        public void UpdatePositionInfo(GenericShip ship)
        {
            Ships[ship].UpdatePositionInfo(new ShipPositionInfo(ship.GetPosition(), ship.GetAngles()));
        }

        public void SwitchToVirtualPosition(GenericShip ship)
        {
            //Add debug mode
            ship.SetPositionInfo(Ships[ship].VirtualPositionInfo);
        }

        public void SwitchToRealPosition(GenericShip ship)
        {
            ship.SetPositionInfo(Ships[ship].RealPositionInfo);
        }

        public void RestoreBoard()
        {
            foreach (var ship in Ships.Keys)
            {
                SwitchToRealPosition(ship);
            }
        }

        public bool IsVirtualPositionReady(GenericShip ship)
        {
            return Ships[ship].VirtualPositionInfo != Ships[ship].RealPositionInfo;
        }
    }
}

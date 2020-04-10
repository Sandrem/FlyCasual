using Ship;
using System;
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
        public string PlannedManeuverCode { get; set; }
        public Dictionary<string, NavigationResult> NavigationResults { get; private set; }
        public int OrderToActivate { get; set; }

        private bool SimpleManeuverPredictionIsReady;
        private bool AllFinalPositionsAreKnown { get { return NavigationResults != null; } }

        private bool VirtualPositionWithCollisionsIsReady;

        public VirtualShipInfo(GenericShip ship)
        {
            Ship = ship;
            RealPositionInfo = new ShipPositionInfo(ship.GetPosition(), ship.GetAngles());
        }

        public void UpdateSimpleManeuverPrediction(ShipPositionInfo virtualPositionInfo, string maneuverCode)
        {
            VirtualPositionInfo = virtualPositionInfo;
            PlannedManeuverCode = maneuverCode;
            SimpleManeuverPredictionIsReady = true;
        }

        public void UpdateNavigationResults(Dictionary<string, NavigationResult> navigationResults)
        {
            NavigationResults = navigationResults;
        }

        public void Clear(ShipPositionInfo positionInfo)
        {
            RealPositionInfo = VirtualPositionInfo = positionInfo;

            SimpleManeuverPredictionIsReady = false;
            NavigationResults = null;
            VirtualPositionWithCollisionsIsReady = false;
        }

        public bool RequiresFinalPositionPrediction()
        {
            return !SimpleManeuverPredictionIsReady && !AllFinalPositionsAreKnown;
        }

        public bool RequiresManeuverAssignment()
        {
            return PlannedManeuverCode == null;
        }

        public void SetPlannedManeuverCode(string maneuverCode, int order)
        {
            PlannedManeuverCode = maneuverCode;
            OrderToActivate = order;
        }

        public bool RequiresCollisionPrediction()
        {
            return VirtualPositionWithCollisionsIsReady == false;
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

        public void SetVirtualPositionInfo(GenericShip ship, ShipPositionInfo virtualPositionInfo, string maneuverCode)
        {
            Ships[ship].UpdateSimpleManeuverPrediction(virtualPositionInfo, maneuverCode);
        }

        public void UpdatePositionInfo(GenericShip ship)
        {
            Ships[ship].Clear(new ShipPositionInfo(ship.GetPosition(), ship.GetAngles()));
        }

        public void SwitchToVirtualPosition(GenericShip ship)
        {
            if (!DebugManager.DebugMovementShowPlanning)
            {
                ShipPositionInfo savedModelPosition = new ShipPositionInfo(ship.GetShipAllPartsTransform().position, ship.GetShipAllPartsTransform().eulerAngles);
                ship.SetPositionInfo(Ships[ship].VirtualPositionInfo);
                ship.GetShipAllPartsTransform().position = savedModelPosition.Position;
                ship.GetShipAllPartsTransform().eulerAngles = savedModelPosition.Angles;
                ship.GetShipAllPartsTransform().Find("ShipBase/ShipBaseCollider").position = Ships[ship].VirtualPositionInfo.Position;
                ship.GetShipAllPartsTransform().Find("ShipBase/ShipBaseCollider").localPosition += new Vector3(0, 0.150289f, 1.156069f);
                ship.GetShipAllPartsTransform().Find("ShipBase/ShipBaseCollider").eulerAngles = Ships[ship].VirtualPositionInfo.Angles;
            }
            else
            {
                ship.SetPositionInfo(Ships[ship].VirtualPositionInfo);
            }
        }

        public void SwitchToRealPosition(GenericShip ship)
        {
            if (!DebugManager.DebugMovementShowPlanning)
            {
                ShipPositionInfo savedModelPosition = new ShipPositionInfo(ship.GetShipAllPartsTransform().position, ship.GetShipAllPartsTransform().eulerAngles);
                ship.SetPositionInfo(Ships[ship].RealPositionInfo);
                ship.GetShipAllPartsTransform().position = savedModelPosition.Position;
                ship.GetShipAllPartsTransform().eulerAngles = savedModelPosition.Angles;
                ship.GetShipAllPartsTransform().Find("ShipBase/ShipBaseCollider").position = savedModelPosition.Position;
                ship.GetShipAllPartsTransform().Find("ShipBase/ShipBaseCollider").eulerAngles = savedModelPosition.Angles;
            }
            else
            {
                ship.SetPositionInfo(Ships[ship].RealPositionInfo);
            }
        }

        public void RestoreBoard()
        {
            foreach (GenericShip ship in Ships.Keys)
            {
                SwitchToRealPosition(ship);
            }
        }

        public void RemoveCollisionsExcept(GenericShip exceptShip)
        {
            foreach (GenericShip ship in Ships.Keys)
            {
                if (ship == exceptShip) continue;

                Vector3 savedModelPosition = ship.GetShipAllPartsTransform().position;
                
                ship.SetPosition(ship.GetPosition() - new Vector3(0, -100, 0));
                ship.GetShipAllPartsTransform().position = savedModelPosition;
            }
        }

        public void ReturnCollisionsExcept(GenericShip exceptShip)
        {
            foreach (GenericShip ship in Ships.Keys)
            {
                if (ship == exceptShip) continue;

                Vector3 savedModelPosition = ship.GetShipAllPartsTransform().position;

                ship.SetPosition(ship.GetPosition() - new Vector3(0, +100, 0));
                ship.GetShipAllPartsTransform().position = savedModelPosition;
            }
        }

        public bool RequiresCollisionPrediction(GenericShip ship)
        {
            return Ships[ship].RequiresCollisionPrediction();
        }

        public bool RequiresFinalPositionPrediction(GenericShip ship)
        {
            return Ships[ship].RequiresFinalPositionPrediction();
        }

        public bool RequiresManeuverAssignment(GenericShip ship)
        {
            return Ships[ship].RequiresManeuverAssignment();
        }

        public void UpdateNavigationResults(GenericShip ship, Dictionary<string, NavigationResult> navigationResults)
        {
            Ships[ship].UpdateNavigationResults(navigationResults);
        }
    }
}

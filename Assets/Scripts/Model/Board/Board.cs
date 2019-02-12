using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using System;

namespace BoardTools
{
    public enum RangeCheckReason
    {
        CoordinateAction,
        UpgradeCard,
        Other
    }

    public static partial class Board {

        public static readonly float PLAYMAT_SIZE = 10;

        public static List<MeshCollider> Objects = new List<MeshCollider>();

        public static void SetShips()
        {
            int i = 1;
            foreach (var ship in Roster.ShipsPlayer1.Values)
            {
                SetShipPreSetup(ship, i);
                RegisterBoardObject(ship);
                i++;
            }

            i = 1;
            foreach (var ship in Roster.ShipsPlayer2.Values)
            {
                SetShipPreSetup(ship, i);
                RegisterBoardObject(ship);
                i++;
            }
        }

        private static void RegisterBoardObject(GenericShip ship)
        {
            Objects.Add(ship.GetShipAllPartsTransform().Find("ShipBase").GetComponent<MeshCollider>());
            Objects.Add(ship.GetShipAllPartsTransform().Find("ShipBase/ObstaclesStayDetector").GetComponent<MeshCollider>());
        }

        public static float CalculateDistance(int countShips)
        {
            float width = 10;
            float distance = width / (countShips + 1);
            return WorldIntoBoard(distance);
        }

        public static void PlaceShip(GenericShip ship, Vector3 position, Vector3 angles, Action callback)
        {
            TurnOffStartingZones();

            ship.SetPosition(position);
            ship.SetAngles(angles);
            ship.IsSetupPerformed = true;

            ship.CallOnShipIsPlaced(callback);
        }

        public static bool IsOffTheBoard(GenericShip ship)
        {
            bool result = false;

            foreach (var obj in ship.ShipBase.GetStandEdgePoints())
            {
                if ((Mathf.Abs(obj.Value.x) > PLAYMAT_SIZE / 2) || (Mathf.Abs(obj.Value.z) > PLAYMAT_SIZE / 2))
                {
                    return true;
                }
            }

            return result;
        }

        public static Direction GetOffTheBoardDirection(GenericShip ship)
        {
            foreach (var obj in ship.ShipBase.GetStandEdgePoints())
            {
                if (obj.Value.x > PLAYMAT_SIZE / 2)
                {
                    return Direction.Right;
                }

                if (obj.Value.x < -PLAYMAT_SIZE / 2)
                {
                    return Direction.Left;
                }

                if (obj.Value.z > PLAYMAT_SIZE / 2)
                {
                    return Direction.Top;
                }

                if (obj.Value.z < -PLAYMAT_SIZE / 2)
                {
                    return Direction.Bottom;
                }
            }

            return Direction.None;
        }

        //SCALING TOOLS

        public static float BoardIntoWorld(float length)
        {
            float scale = 10 / SIZE_X;
            return length * scale;
        }

        public static float WorldIntoBoard(float length)
        {
            float scale = SIZE_X / 10;
            return length * scale;
        }

        public static void Cleanup()
        {
            Objects = new List<MeshCollider>();
        }

        public static bool CheckInRange(GenericShip thisShip, GenericShip anotherShip, int minRange, int maxRange, RangeCheckReason reason = RangeCheckReason.Other)
        {
            DistanceInfo distInfo = new DistanceInfo(thisShip, anotherShip);
            bool inRange = distInfo.Range >= minRange && distInfo.Range <= maxRange;

            inRange = thisShip.CallOnCheckRange(anotherShip, minRange, maxRange, reason, inRange);

            return inRange;
        }
    
    }

}

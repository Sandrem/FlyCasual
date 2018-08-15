using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using System;
using Players;
using System.Linq;

namespace BoardTools
{

    public static partial class Board {

        public static readonly float PLAYMAT_SIZE = 10;

        public static List<MeshCollider> Objects = new List<MeshCollider>();

        public static void SetShips()
        {
            SetShips(Roster.Player1);
            SetShips(Roster.Player2);
        }

        private static void SetShips(GenericPlayer player)
        {
            if (player is HotacAiPlayer)
            {
                var shipGroups = player.Ships.Values.GroupBy(ship => 
                    ship.Formation != null 
                    ? "F:" + ship.Formation.Name
                    : "S:" + ship.ShipId.ToString());

                int i = 1;
                foreach (var group in shipGroups)
                {
                    if (group.Count() == 1)
                    {
                        SetShipPreSetup(group.First(), i, shipGroups.Count());
                    }
                    else
                    {
                        group.First().Formation.SetFormationPreSetup(i, shipGroups.Count());
                    }
                    i++;
                }
            }
            else
            {
                int i = 1;
                foreach (var ship in player.Ships)
                {
                    SetShipPreSetup(ship.Value, i, ship.Value.Owner.Ships.Count);
                    i++;
                }
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
    
    }

}

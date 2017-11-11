using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Board
{
    public static partial class BoardManager
    {

        public static Transform BoardTransform;

        public static Transform RulersHolderTransform;

        public static GameObject StartingZone1;
        public static GameObject StartingZone2;

        public static readonly float SIZE_ANY = 91.44f;
        public static readonly float SIZE_X = 91.44f;
        public static readonly float SIZE_Y = 91.44f;
        public static readonly float SHIP_STAND_SIZE = 4f;
        public static readonly float DISTANCE_1 = 4f;
        public static readonly float RANGE_1 = 10f;

        public static void Initialize()
        {
            BoardTransform = GameObject.Find("SceneHolder/Board").transform;
            RulersHolderTransform = BoardTransform.Find("RulersHolder");
            StartingZone1 = BoardTransform.Find("Playmat/StaringZone1").gameObject;
            StartingZone2 = BoardTransform.Find("Playmat/StaringZone2").gameObject;

            MovementTemplates.PrepareMovementTemplates();

            SetPlaymatImage();
        }

        private static void SetPlaymatImage()
        {
            if (!string.IsNullOrEmpty(Options.Playmat))
            {
                Texture playmatTexture = (Texture)Resources.Load("Playmats/Playmat" + Options.Playmat + "Texture", typeof(Texture));
                BoardTransform.Find("Playmat").GetComponent<Renderer>().material.mainTexture = playmatTexture;
            }
        }

        private static void SetShip(Ship.GenericShip ship, int count)
        {
            float distance = CalculateDistance(ship.Owner.Ships.Count);
            float side = (ship.Owner.PlayerNo == Players.PlayerNo.Player1) ? -1 : 1;
            ship.SetPosition(BoardIntoWorld(new Vector3(-SIZE_X / 2 + count * distance, 0, side * SIZE_Y / 2 + +side * 2 * RANGE_1)));
            ship.SetPosition(BoardIntoWorld(new Vector3(-SIZE_X / 2 + count * distance, 0, side * SIZE_Y / 2 + -side * 2 * RANGE_1)));
        }

        public static void HighlightStartingZones()
        {
            StartingZone1.SetActive(Phases.CurrentPhasePlayer == Players.PlayerNo.Player1);
            StartingZone2.SetActive(Phases.CurrentPhasePlayer == Players.PlayerNo.Player2);
        }

        public static void TurnOffStartingZones()
        {
            StartingZone1.SetActive(false);
            StartingZone2.SetActive(false);
        }

        public static void SetShips(Dictionary<string, Ship.GenericShip> shipsPlayer1, Dictionary<string, Ship.GenericShip> shipsPlayer2)
        {

            int i = 1;
            foreach (var ship in shipsPlayer1)
            {
                float distance = CalculateDistance(shipsPlayer1.Count);
                ship.Value.SetPosition(BoardIntoWorld(new Vector3(- SIZE_X / 2 + i *distance, 0, -SIZE_Y/2 + 2*RANGE_1)));
                i++;
            }

            i = 1;
            foreach (var ship in shipsPlayer2)
            {
                float distance = CalculateDistance(shipsPlayer2.Count);
                ship.Value.SetPosition(BoardIntoWorld(new Vector3(- SIZE_X / 2 + i * distance, 0, SIZE_Y/2 - 2*RANGE_1)));
                i++;
            }
        }

        //SCALING TOOLS

        public static Vector3 BoardIntoWorld(Vector3 position)
        {
            return BoardTransform.TransformPoint(position);
        }

        public static Vector3 WorldIntoBoard(Vector3 position)
        {
            return BoardTransform.InverseTransformPoint(position);
        }

        //GET TRANSFORMS

        public static Transform GetBoard()
        {
            return BoardTransform;
        }

        public static Transform GetRulersHolder()
        {
            return RulersHolderTransform;
        }

        public static Transform GetStartingZone(Players.PlayerNo playerNo)
        {
            return (playerNo == Players.PlayerNo.Player1) ? StartingZone1.transform : StartingZone2.transform;
        }

        public static bool ShipStandIsInside(GameObject shipObject, Transform zone)
        {
            Vector3 zoneStart = zone.transform.TransformPoint(-0.5f, -0.5f, -0.5f);
            Vector3 zoneEnd = zone.transform.TransformPoint(0.5f, 0.5f, 0.5f);
            bool result = true;

            List<Vector3> shipStandEdges = new List<Vector3>
            {
                shipObject.transform.TransformPoint(new Vector3(-0.5f, 0f, 0)),
                shipObject.transform.TransformPoint(new Vector3(0.5f, 0f, 0)),
                shipObject.transform.TransformPoint(new Vector3(-0.5f, 0f, -1f)),
                shipObject.transform.TransformPoint(new Vector3(0.5f, 0f, -1))
            };

            foreach (var point in shipStandEdges)
            {
                if ((point.x < zoneStart.x) || (point.z < zoneStart.z) || (point.x > zoneEnd.x) || (point.z > zoneEnd.z))
                {
                    result = false;
                    break;
                }
            }
            return result;
        }

        public static int GetRangeBetweenPoints(Vector3 pointA, Vector3 pointB)
        {
            int result = 0;

            Vector3 boardPointA = WorldIntoBoard(pointA);
            Vector3 boardPointB = WorldIntoBoard(pointB);

            result = Mathf.CeilToInt(Vector3.Distance(boardPointA, boardPointB) / RANGE_1);

            return result;
        }

    }

}

﻿using System;
using System.Collections;
using System.Collections.Generic;
using Arcs;
using Ship;
using UnityEngine;

namespace BoardTools
{
    public static partial class Board
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

        //TODO: Rework
        public static readonly float DISTANCE_INTO_RANGE = 3.28f / 3f;

        public static void Initialize()
        {
            BoardTransform = GameObject.Find("SceneHolder/Board").transform;
            RulersHolderTransform = BoardTransform.Find("RulersHolder");
            StartingZone1 = BoardTransform.Find("Playmat/StaringZone1").gameObject;
            StartingZone2 = BoardTransform.Find("Playmat/StaringZone2").gameObject;

            MovementTemplates.PrepareMovementTemplates();

            SetPlaymatScene();
            SetObstacles();
        }

        private static void SetPlaymatScene()
        {
            if (Options.Playmat.StartsWith("3DScene"))
            {
                SetScene3D(Options.Playmat);
            }
            else
            {
                SetPlaymat(Options.Playmat);
            }
        }

        private static void SetPlaymat(string playmatName)
        {
            LoadSceneFromResources("TableClassic");

            Texture playmatTexture = (Texture)Resources.Load("Playmats/Playmat" + Options.Playmat + "Texture", typeof(Texture));
            GameObject.Find("SceneHolder/TableClassic/Playmat").GetComponent<Renderer>().material.mainTexture = playmatTexture;

            RenderSettings.fog = false;
        }

        private static void SetScene3D(string sceneNameFull)
        {
            string sceneName = sceneNameFull.Replace("3DScene", "");
            LoadSceneFromResources(sceneName);

            RenderSettings.fog = true;

            GameObject.Find("SceneHolder/Board/").transform.Find("CombatDiceHolder").transform.position += new Vector3(0, 100, 0);
            GameObject.Find("SceneHolder/Board/").transform.Find("CheckDiceHolder").transform.position += new Vector3(0, 100, 0);
            GameObject.Find("SceneHolder/Board/").transform.Find("RulersHolder").transform.position += new Vector3(0, 100, 0);
        }

        private static void LoadSceneFromResources(string sceneName)
        {
            GameObject scenePartPrefab = (GameObject)Resources.Load("Prefabs/Scenes/" + sceneName);
            GameObject scenePart  = MonoBehaviour.Instantiate(scenePartPrefab, GameObject.Find("SceneHolder").transform);
            scenePart.name = sceneName;

            Material skyboxMaterial = (Material)Resources.Load("Prefabs/Skyboxes/" + sceneName);
            RenderSettings.skybox = skyboxMaterial;
            DynamicGI.UpdateEnvironment();
        }

        private static void SetShipPreSetup(GenericShip ship, int count)
        {
            float distance = CalculateDistance(ship.Owner.Ships.Count);
            float side = (ship.Owner.PlayerNo == Players.PlayerNo.Player1) ? -1 : 1;
            ship.SetPosition(
                BoardIntoWorld(
                    new Vector3(-SIZE_X / 2 + count * distance, 0, side * (SIZE_Y / 2 + DISTANCE_1))
                )
            );

            RegisterBoardObject(ship);
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

        //util functions
        public static int GetRangeOfShips(GenericShip from, GenericShip to)
        {
            DistanceInfo positionInfo = new DistanceInfo(from, to);
            return positionInfo.Range;
        }

        public static bool IsShipAtRange(GenericShip from, GenericShip to, int range)
        {
            return IsShipBetweenRange(from, to, range, range);
        }

        public static bool IsShipBetweenRange(GenericShip from, GenericShip to, int minrange, int maxrange)
        {
            if(minrange == maxrange && minrange == 0)
            {
                if (from.ShipsBumped.Contains(to) || from == to)
                    return true;

                return false;
            }

            int range = GetRangeOfShips(from, to);

            if (range >= minrange && range <= maxrange)
                return true;

            return false;
        }

        public static bool IsShipInArc(GenericShip source, GenericShip target)
        {
            ShotInfo shotInfo = new ShotInfo(source, target, source.PrimaryWeapon);
            return shotInfo.InArc;
        }

        public static bool IsShipInArcByType(GenericShip source, GenericShip target, ArcTypes arc)
        {
            ShotInfo shotInfo = new ShotInfo(source, target, source.PrimaryWeapon);
            return shotInfo.InArcByType(arc);
        }

        public static List<GenericShip> GetShipsInBullseyeArc(GenericShip ship, Team.Type team = Team.Type.Any)
        {
            List<GenericShip> shipsInBullseyeArc = new List<GenericShip>();
            foreach(var kv in Roster.AllShips)
            {
                GenericShip otherShip = kv.Value;

                if (team == Team.Type.Friendly && ship.Owner.Id != otherShip.Owner.Id)
                    continue;

                if (team == Team.Type.Enemy && ship.Owner.Id == otherShip.Owner.Id)
                    continue;

                ShotInfo shotInfo = new ShotInfo(ship, otherShip, ship.PrimaryWeapon);
                if (!shotInfo.InArcByType(ArcTypes.Bullseye))
                    continue;

                shipsInBullseyeArc.Add(otherShip);
            }

            return shipsInBullseyeArc;
        }

        public static List<GenericShip> GetShipsAtRange(GenericShip ship, Vector2 fromto, Team.Type team = Team.Type.Any)
        {
            List<GenericShip> ships = new List<GenericShip>();
            foreach (var kv in Roster.AllShips)
            {
                GenericShip othership = kv.Value;

                if (team == Team.Type.Friendly && ship.Owner.Id != othership.Owner.Id)
                    continue;

                if (team == Team.Type.Enemy && ship.Owner.Id == othership.Owner.Id)
                    continue;

                int range = GetRangeOfShips(ship, othership);
                if (range >= fromto.x && range <= fromto.y)
                {
                    ships.Add(othership);
                }
            }

            return ships;
        }

        private static void SetObstacles()
        {
            for (int i = 1; i <= 6; i++)
            {
                Objects.Add(GameObject.Find("SceneHolder/Board/ObstaclesHolder/A" + i + "/A" + i + "model").GetComponent<MeshCollider>());
            }
        }

        public static void ToggleObstaclesHolder(bool isActive)
        {
            BoardTransform.Find("ObstaclesHolder").gameObject.SetActive(isActive);
        }

        public static void ToggleDiceHolders(bool isActive)
        {
            BoardTransform.Find("CombatDiceHolder").gameObject.SetActive(isActive);
            BoardTransform.Find("CheckDiceHolder").gameObject.SetActive(isActive);
        }

        public static void ToggleOffTheBoardHolder(bool isActive)
        {
            BoardTransform.Find("OffTheBoardHolder").gameObject.SetActive(isActive);
        }
    }
}

namespace Team
{
    public enum Type
    {
         Friendly,
         Enemy,
         Any
    }
}
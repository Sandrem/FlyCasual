using BoardTools;
using Players;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Obstacles
{
    public class ObstaclesManager
    {
        public static ObstaclesManager Instance;

        public List<GenericObstacle> AllPossibleObstacles { get; private set; }
        public List<GenericObstacle> ChosenObstacles { get; set; }

        public ObstaclesManager()
        {
            Instance = this;

            InitializeObstacles();
        }

        private static void InitializeObstacles()
        {
            Instance.AllPossibleObstacles = new List<GenericObstacle>();
            for (int i = 0; i < 6; i++)
            {
                Instance.AllPossibleObstacles.Add(
                    new Asteroid(
                        "Core Asteroid " + i,
                        "coreasteroid" + i
                    )
                );

                Instance.AllPossibleObstacles.Add(
                    new Asteroid(
                        "Force Awakens Asteroid " + i,
                        "core2asteroid" + i
                    )
                );
            }

            // TODO: Add all debris
            Instance.AllPossibleObstacles.Add(
                new Debris(
                    "YT2400 Debris 2",
                    "yt2400debris2"
                )
            );

            for (int i = 1; i < 4; i++)
            {
                Instance.AllPossibleObstacles.Add(
                    new GasCloud(
                        "Gas Cloud " + i,
                        "gascloud" + i
                    )
                );
            }

            Instance.AllPossibleObstacles = Instance.AllPossibleObstacles.OrderBy(n => n.Name).ToList();
        }

        public static GenericObstacle GenerateObstacle(string shortName, PlayerNo playerNo)
        {
            GenericObstacle prefab = Instance.AllPossibleObstacles.First(n => n.ShortName == shortName);
            GenericObstacle newObstacle = (GenericObstacle)Activator.CreateInstance(
                prefab.GetType(),
                new object[] { prefab.Name, prefab.ShortName }
            );
            Instance.ChosenObstacles.Add(newObstacle);            
            return newObstacle;
        }

        public static GenericObstacle GetChosenObstacle(string name)
        {
            return Instance.ChosenObstacles.First(n => n.Name == name);
        }

        public static GenericObstacle GetPossibleObstacle(string obstacleShortName)
        {
            return Instance.AllPossibleObstacles.First(n => n.ShortName == obstacleShortName);
        }

        // OLD

        public static List<GenericObstacle> GetPlacedObstacles()
        {
            return Instance.ChosenObstacles.Where(n => n.IsPlaced).ToList();
        }

        public static int GetPlacedObstaclesCount()
        {
            return Instance.ChosenObstacles.Count(n => n.IsPlaced);
        }

        public static GenericObstacle GetRandomFreeObstacle()
        {
            List<GenericObstacle> freeObstacles = Instance.ChosenObstacles.Where(n => !n.IsPlaced).ToList();
            int random = UnityEngine.Random.Range(0, freeObstacles.Count);
            return freeObstacles[random];
        }

        public static void DestroyObstacle(GenericObstacle obstacle)
        {
            foreach (GenericShip ship in Roster.AllShips.Values)
            {
                ship.LandedOnObstacles.RemoveAll(n => n == obstacle);
            }

            Instance.ChosenObstacles.Remove(obstacle);
            Board.Objects.Remove(obstacle.ObstacleGO.GetComponentInChildren<MeshCollider>());
            GameObject.Destroy(obstacle.ObstacleGO);
        }
    }
}

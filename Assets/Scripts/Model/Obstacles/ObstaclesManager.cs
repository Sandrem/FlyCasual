using BoardTools;
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

        public List<GenericObstacle> Obstacles { get; private set; }

        public ObstaclesManager()
        {
            Instance = this;

            InitializeObstacles();
        }

        private static void InitializeObstacles()
        {
            Instance.Obstacles = new List<GenericObstacle>();
            for (int i = 0; i < 6; i++)
            {
                Instance.Obstacles.Add(new Asteroid("Core Asteroid " + i));
                Instance.Obstacles.Add(new Asteroid("Force Awakens Asteroid " + i));
            }

            // TODO: Add all debris
            Instance.Obstacles.Add(new Debris("YT2400 Debris 2"));

            for (int i = 1; i < 4; i++)
            {
                Instance.Obstacles.Add(new GasCloud("Gas Cloud " + i));
            }

            Instance.Obstacles = Instance.Obstacles.OrderBy(n => n.Name).ToList();
        }

        // OLD

        public static GenericObstacle GetObstacleByName(string obstacleName)
        {
            return Instance.Obstacles.First(n => n.ObstacleGO.name == obstacleName);
        }

        public static GenericObstacle GetObstacleByTransform(Transform transform)
        {
            GameObject obstacleGO = transform.parent.gameObject;
            return GetObstacleByName(obstacleGO.name);
        }

        public static List<GenericObstacle> GetPlacedObstacles()
        {
            return Instance.Obstacles.Where(n => n.IsPlaced).ToList();
        }

        public static int GetPlacedObstaclesCount()
        {
            return Instance.Obstacles.Count(n => n.IsPlaced);
        }

        public static GenericObstacle GetRandomFreeObstacle()
        {
            List<GenericObstacle> freeObstacles = Instance.Obstacles.Where(n => !n.IsPlaced).ToList();
            int random = UnityEngine.Random.Range(0, freeObstacles.Count);
            return freeObstacles[random];
        }

        public static void DestroyObstacle(GenericObstacle obstacle)
        {
            foreach (GenericShip ship in Roster.AllShips.Values)
            {
                ship.LandedOnObstacles.RemoveAll(n => n == obstacle);
            }

            Instance.Obstacles.Remove(obstacle);
            Board.Objects.Remove(obstacle.ObstacleGO.GetComponentInChildren<MeshCollider>());
            GameObject.Destroy(obstacle.ObstacleGO);
        }
    }
}

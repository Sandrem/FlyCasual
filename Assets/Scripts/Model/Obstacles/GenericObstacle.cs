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
    public abstract class GenericObstacle
    {
        public string Name { get; set; }
        public string ShortName { get; protected set; }
        public bool IsPlaced { get; set; }
        public GameObject ObstacleGO { get; set; }

        public GenericObstacle(string name, string shortName)
        {
            Name = name;
            ShortName = shortName;
        }

        public abstract string GetTypeName { get; }

        public abstract void OnHit(GenericShip ship);
        public abstract void OnLanded(GenericShip ship);
        public abstract void OnShotObstructedExtra(GenericShip attacker, GenericShip defender);

        public void Spawn(string name, Transform obstacleHolder)
        {
            GameObject obstacleModelPrefab = Resources.Load<GameObject>(string.Format("Prefabs/Obstacles/{0}/{1}", GetTypeName, Name));
            ObstacleGO = GameObject.Instantiate<GameObject>(obstacleModelPrefab, obstacleHolder);
            Name = name;
            ObstacleGO.name = name;
            ObstacleGO.transform.Find("default").name = name;
            Board.RegisterObstacle(this);
        }
    }
}

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
        public string Name { get; protected set; }
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
        public abstract void OnShotObstructed(GenericShip attacker, GenericShip defender);
    }
}

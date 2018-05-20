using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Obstacles
{
    public class Asteroid: GenericObstacle
    {
        public Asteroid(GameObject obstacleGO) : base(obstacleGO)
        {
            Name = "Asteroid";
        }
    }
}

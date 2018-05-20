using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Obstacles
{
    public class Debris: GenericObstacle
    {
        public Debris(GameObject obstacleGO) : base(obstacleGO)
        {
            Name = "Debris";
        }
    }
}

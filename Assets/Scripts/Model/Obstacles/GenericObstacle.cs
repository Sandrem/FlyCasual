﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Obstacles
{
    public class GenericObstacle
    {
        public string Name { get; protected set; }
        public bool IsPlaced { get; set; }
        public GameObject ObstacleGO { get; set; }

        public GenericObstacle(GameObject obstacleGO)
        {
            ObstacleGO = obstacleGO;
        }
    }
}

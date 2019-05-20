using Obstacles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclesHitsDetector : MonoBehaviour {

    public bool checkCollisions = false;

    public List<GenericObstacle> OverlapedAsteroids = new List<GenericObstacle>();
    public List<Collider> OverlapedMines = new List<Collider>();

    void OnTriggerEnter(Collider collisionInfo)
    {
        if (checkCollisions)
        {
            if (collisionInfo.tag == "Obstacle")
            {
                GenericObstacle obstacle = ObstaclesManager.GetChosenObstacle(collisionInfo.transform.name);
                if (!OverlapedAsteroids.Contains(obstacle))
                {
                    OverlapedAsteroids.Add(obstacle);
                }
            }

            if (collisionInfo.tag == "Mine")
            {
                if (!OverlapedMines.Contains(collisionInfo))
                {
                    OverlapedMines.Add(collisionInfo);
                }
            }
        }
    }

}

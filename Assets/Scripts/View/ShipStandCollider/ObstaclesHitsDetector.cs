using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclesHitsDetector : MonoBehaviour {

    public bool checkCollisions = false;

    public List<Collider> OverlapedAsteroids = new List<Collider>();

    void OnTriggerEnter(Collider collisionInfo)
    {
        if (checkCollisions)
        {
            if (collisionInfo.tag == "Asteroid")
            {
                if (!OverlapedAsteroids.Contains(collisionInfo))
                {
                    OverlapedAsteroids.Add(collisionInfo);
                }
            }
        }
    }

}

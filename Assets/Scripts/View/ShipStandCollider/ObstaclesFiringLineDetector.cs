using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclesFiringLineDetector : MonoBehaviour {

    void OnTriggerEnter(Collider collisionInfo)
    {
        if (collisionInfo.tag == "Asteroid")
        {
            Board.FiringLineCollisions.Add(collisionInfo);
        }
    }

}

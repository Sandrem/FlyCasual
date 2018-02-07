using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclesFiringLineDetector : MonoBehaviour {

    public bool IsObstructedByAsteroid { get; set; }
    public bool IsObstructedByBombToken { get; set; }

    public Vector3 PointStart;
    public Vector3 PointEnd;

    void OnTriggerEnter(Collider collisionInfo)
    {
        if (collisionInfo.tag == "Asteroid")
        {
            IsObstructedByAsteroid = true;
        }
        else if (collisionInfo.tag == "Mine" || collisionInfo.tag == "TimedBomb")
        {
            IsObstructedByBombToken = true;
        }
    }

}

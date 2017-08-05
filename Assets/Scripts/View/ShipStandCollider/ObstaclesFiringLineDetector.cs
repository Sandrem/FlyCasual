using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclesFiringLineDetector : MonoBehaviour {

    public bool IsObstructed { get; set; }

    public Vector3 PointStart;
    public Vector3 PointEnd;

    void OnTriggerEnter(Collider collisionInfo)
    {
        if (collisionInfo.tag == "Asteroid")
        {
            IsObstructed = true;
        }
    }

}

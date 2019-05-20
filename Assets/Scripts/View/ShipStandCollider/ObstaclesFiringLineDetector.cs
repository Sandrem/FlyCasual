using Obstacles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclesFiringLineDetector : MonoBehaviour {

    public bool IsObstructedByAsteroid { get { return ObstaclesObstructed.Count > 0; } }
    public bool IsObstructedByBombToken { get; set; }
    public List<GenericObstacle> ObstaclesObstructed = new List<GenericObstacle>();

    public Vector3 PointStart;
    public Vector3 PointEnd;

    void OnTriggerEnter(Collider collisionInfo)
    {
        if (collisionInfo.tag == "Obstacle")
        {
            ObstaclesObstructed.Add(ObstaclesManager.GetChosenObstacle(collisionInfo.transform.name));
        }
        else if (collisionInfo.tag == "Mine" || collisionInfo.tag == "TimedBomb")
        {
            IsObstructedByBombToken = true;
        }
    }

}

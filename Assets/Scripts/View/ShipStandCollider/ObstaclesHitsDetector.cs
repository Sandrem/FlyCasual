using Obstacles;
using Remote;
using Ship;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclesHitsDetector : MonoBehaviour {

    public bool checkCollisions = false;

    public List<GenericObstacle> OverlapedAsteroids = new List<GenericObstacle>();
    public List<Collider> OverlapedMines = new List<Collider>();
    public List<GenericRemote> RemotesMovedThrough = new List<GenericRemote>();
    public List<GenericShip> ShipsMovedThrough = new List<GenericShip>();

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
            else if(collisionInfo.tag == "Mine")
            {
                if (!OverlapedMines.Contains(collisionInfo))
                {
                    OverlapedMines.Add(collisionInfo);
                }
            }
            else if (collisionInfo.name == "RemoteCollider")
            {
                if (collisionInfo.tag != this.tag)
                {
                    if (!RemotesMovedThrough.Contains(Roster.GetShipById(collisionInfo.tag) as GenericRemote))
                    {
                        RemotesMovedThrough.Add(Roster.GetShipById(collisionInfo.tag) as GenericRemote);
                    }
                }
            }
            else if (collisionInfo.name == "ObstaclesStayDetector")
            {
                if (collisionInfo.tag != this.tag)
                {
                    if (!ShipsMovedThrough.Contains(Roster.GetShipById(collisionInfo.tag)))
                    {
                        ShipsMovedThrough.Add(Roster.GetShipById(collisionInfo.tag));
                    }
                }
            }
        }
    }

}

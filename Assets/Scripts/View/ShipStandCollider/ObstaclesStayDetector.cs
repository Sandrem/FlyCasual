using Obstacles;
using Remote;
using Ship;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclesStayDetector: MonoBehaviour {

    public bool checkCollisions = false;

    public bool OverlapsShip = false;

    public List<GenericShip> OverlapedShips = new List<GenericShip>();
    public List<GenericRemote> OverlapedRemotes = new List<GenericRemote>();
    public List<GenericObstacle> OverlapedAsteroids = new List<GenericObstacle>();
    public List<Collider> OverlapedMines = new List<Collider>();

    public bool OffTheBoard = false;

    void OnTriggerEnter(Collider collisionInfo)
    {
        if (checkCollisions)
        {
            GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            if (collisionInfo.tag == "Obstacle")
            {
                GenericObstacle obstacle = ObstaclesManager.GetChosenObstacle(collisionInfo.transform.name);
                if (!OverlapedAsteroids.Contains(obstacle))
                {
                    OverlapedAsteroids.Add(obstacle);
                }
            }
            else if (collisionInfo.tag == "Mine")
            {
                if (!OverlapedMines.Contains(collisionInfo))
                {
                    OverlapedMines.Add(collisionInfo);
                }
            }
            else if (collisionInfo.name == "OffTheBoard")
            {
                OffTheBoard = true;
            }
            else if (collisionInfo.name == "ObstaclesStayDetector")
            {
                if (collisionInfo.tag != this.tag)
                {
                    Game.Movement.CollidedWith = collisionInfo;
                    OverlapsShip = true;
                    if (!OverlapedShips.Contains(Roster.GetShipById(collisionInfo.tag)))
                    {
                        OverlapedShips.Add(Roster.GetShipById(collisionInfo.tag));
                    }
                }
            }
            else if (collisionInfo.name == "RemoteCollider")
            {
                if (collisionInfo.tag != this.tag)
                {
                    if (!OverlapedRemotes.Contains(Roster.GetShipById(collisionInfo.tag) as GenericRemote))
                    {
                        OverlapedRemotes.Add(Roster.GetShipById(collisionInfo.tag) as GenericRemote);
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider collisionInfo)
    {
        if (checkCollisions)
        {
            if (collisionInfo.name == "ObstaclesStayDetector")
            {
                if (collisionInfo.tag != this.tag)
                {
                    if (OverlapedShips.Contains(Roster.GetShipById(collisionInfo.tag)))
                    {
                        OverlapedShips.Remove(Roster.GetShipById(collisionInfo.tag));
                    }
                }
            }
        }
    }

}

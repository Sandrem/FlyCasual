using Obstacles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclesStayDetector: MonoBehaviour {

    public bool checkCollisions = false;

    public bool OverlapsShip = false;
    public List<Ship.GenericShip> OverlapedShips = new List<Ship.GenericShip>();

    public List<GenericObstacle> OverlapedAsteroids = new List<GenericObstacle>();

    public List<Collider> OverlapedMines = new List<Collider>();

    public bool OffTheBoard = false;

    void OnTriggerEnter(Collider collisionInfo)
    {
        if (checkCollisions)
        {
            GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            if (collisionInfo.tag == "Asteroid")
            {
                GenericObstacle obstacle = ObstaclesManager.GetObstacleByTransform(collisionInfo.transform);
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclesStayDetector: MonoBehaviour {

    private GameManagerScript Game;

    public bool checkCollisions = false;

    public bool OverlapsShip = false;
    public List<Ship.GenericShip> OverlapedShips = new List<Ship.GenericShip>();

    public bool OverlapsAsteroid = false;
    public List<Collider> OverlapedAsteroids = new List<Collider>();

    // Use this for initialization
    void Start () {
        Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
    }
	
	// Update is called once per frame
	void Update () {

	}

    void OnTriggerEnter(Collider collisionInfo)
    {
        if (checkCollisions)
        {
            if (collisionInfo.tag == "Asteroid")
            {
                Game.Movement.ObstacleEnter = collisionInfo;
                OverlapsAsteroid = true;
                if (!OverlapedAsteroids.Contains(collisionInfo))
                {
                    OverlapedAsteroids.Add(collisionInfo);
                }
            }
            if (collisionInfo.name == "ObstaclesStayDetector")
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

    void OnTriggerStay(Collider collisionInfo)
    {
        //TODO: Change to OnTriggerEnter
        /*if (checkCollisions)
        {
            if (collisionInfo.name == "ObstaclesStayDetector")
            {
                if (collisionInfo.tag != this.tag)
                {
                    Game.Movement.CollidedWith = collisionInfo;
                }
            }
        }*/
    }

    void OnTriggerExit(Collider collisionInfo)
    {
        /*if (checkCollisions)
        {
            if (collisionInfo.name == "ObstaclesStayDetector")
            {
                Game.Movement.CollidedWith = null;
                Selection.ThisShip.LastShipCollision = Roster.GetShipById(collisionInfo.tag);
            }

            if (collisionInfo.tag == "Asteroid")
            {
                Game.Movement.ObstacleExit = collisionInfo;
                OverlapsAsteroid = false;
            }
        }*/
    }

}

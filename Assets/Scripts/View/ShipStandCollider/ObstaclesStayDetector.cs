using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclesStayDetector: MonoBehaviour {

    private GameManagerScript Game;

    public bool checkCollisions = false;
    public bool checkCollisionsNow = false;

    public bool OverlapsShip = false;
    public bool OverlapsShipNow = false;
    public List<Ship.GenericShip> OverlapedShips = new List<Ship.GenericShip>();

    public bool OverlapsAsteroid = false;
    public bool OverlapsAsteroidNow = false;
    public List<Collider> OverlapedAsteroids = new List<Collider>();

    public bool OffTheBoard = false;
    public bool OffTheBoardNow = false;

    // Use this for initialization
    void Start () {
        Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
    }
	
	// Update is called once per frame
	void Update ()
    {

    }

    void OnTriggerEnter(Collider collisionInfo)
    {
        if (checkCollisions)
        {
            if (collisionInfo.tag == "Asteroid")
            {
                //Temporary
                if (Game == null) Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

                Game.Movement.ObstacleEnter = collisionInfo;
                OverlapsAsteroid = true;
                if (!OverlapedAsteroids.Contains(collisionInfo))
                {
                    OverlapedAsteroids.Add(collisionInfo);
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

    public void ReCheckCollisionsStart()
    {
        OverlapsAsteroidNow = false;
        OverlapsShipNow = false;
        OffTheBoardNow = false;

        checkCollisionsNow = true;
    }

    public void ReCheckCollisionsFinish()
    {
        checkCollisionsNow = false;
    }

    void OnTriggerStay(Collider collisionInfo)
    {
        if (checkCollisionsNow)
        {
            if (collisionInfo.tag == "Asteroid")
            {
                OverlapsAsteroidNow = true;
            }
            else if (collisionInfo.name == "OffTheBoard")
            {
                OffTheBoardNow = true;
            }
            else if (collisionInfo.name == "ObstaclesStayDetector")
            {
                if (collisionInfo.tag != this.tag)
                {
                    OverlapsShipNow = true;
                }
            }
        }
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

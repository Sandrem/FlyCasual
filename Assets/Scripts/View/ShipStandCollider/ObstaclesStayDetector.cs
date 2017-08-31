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

    public bool OffTheBoard = false;

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

    void OnTriggerStay(Collider collisionInfo)
    {

    }

    void OnTriggerExit(Collider collisionInfo)
    {

    }

}

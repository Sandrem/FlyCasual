using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclesStayDetector: MonoBehaviour {

    private GameManagerScript Game;

    public bool checkCollisions = false;

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
            }
        }
    }

    void OnTriggerStay(Collider collisionInfo)
    {
        //TODO: Change to OnTriggerEnter
        if (checkCollisions)
        {
            if (collisionInfo.name == "ShipStand")
            {
                Game.Movement.CollidedWith = collisionInfo;
            }
        }
    }

    void OnTriggerExit(Collider collisionInfo)
    {
        if (checkCollisions)
        {
            if (collisionInfo.name == "ShipStand")
            {
                Game.Movement.CollidedWith = null;
                Selection.ThisShip.LastShipCollision = Roster.GetShipById(collisionInfo.tag);
            }

            if (collisionInfo.tag == "Asteroid")
            {
                Game.Movement.ObstacleExit = collisionInfo;
            }
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclesHitsDetector : MonoBehaviour {

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
                Game.Movement.ObstacleHitEnter = collisionInfo;
            }
        }
    }

}

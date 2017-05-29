using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: One object with 6 meshes???
public class AsteroidCollisionScript : MonoBehaviour {

    private GameManagerScript Game;

    // Use this for initialization
    void Start () {
        Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider collisionInfo)
    {
        if (collisionInfo.tag.StartsWith("ShipId:"))
        {
            Game.Movement.ObstacleEnter = this.gameObject.GetComponent<Collider>();
        }
	}

    void OnTriggerExit(Collider collisionInfo)
    {
        if (collisionInfo.tag.StartsWith("ShipId:"))
        {
            Game.Movement.ObstacleExit = this.gameObject.GetComponent<Collider>();
        }
    }

}

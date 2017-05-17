using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipStandCollider: MonoBehaviour {

    private GameManagerScript Game;

    public bool checkCollisions = false;

	// Use this for initialization
	void Start () {
        Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerStay(Collider collisionInfo) {
		if (checkCollisions) {
			if (collisionInfo.name == "ShipStand") {
				Game.Movement.CollidedWith = collisionInfo;
			}
		}
	}

	void OnTriggerExit(Collider collisionInfo) {
		if (checkCollisions) {
			if (collisionInfo.name == "ShipStand") {
                Game.Movement.CollidedWith  = null;
				Game.Selection.ThisShip.LastShipCollision = Game.Roster.GetShipByTag (collisionInfo.tag);
			}
		}
	}

}

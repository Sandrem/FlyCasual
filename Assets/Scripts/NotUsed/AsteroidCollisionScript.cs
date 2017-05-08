using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: One object with 6 meshes???
public class AsteroidCollisionScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider collisionInfo) {
		//TODO: If movement reverted?
		if (collisionInfo.name == "MovementRuler") {
			//TODO: Check damage
			Debug.Log("Asteroid collision, check damage to the ship");
		}
		//TODO: Check if ship stays on asteroid
	}

}

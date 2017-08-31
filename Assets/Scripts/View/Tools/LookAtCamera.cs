using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour {

    GameManagerScript Game;

    private void Start()
    {
        Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
    }

    // Update is called once per frame
    void Update ()
    {
        gameObject.GetComponent<Renderer>().enabled = Game.UI.ShowShipIds;
        if (Game.UI.ShowShipIds)
        {
            transform.forward = GameObject.Find("CameraHolder/Main Camera").transform.forward;
        }
	}
}

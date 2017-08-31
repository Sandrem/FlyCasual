using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewByAlt : MonoBehaviour {

    GameManagerScript Game;

    private void Start()
    {
        Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
    }

    void Update ()
    {
        gameObject.GetComponent<Canvas>().enabled = Game.UI.ShowShipIds;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceOverlayPositionUpdater : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (gameObject.activeSelf)
        {
            transform.position = transform.parent.Find("Dice").position;
        }
	}
}

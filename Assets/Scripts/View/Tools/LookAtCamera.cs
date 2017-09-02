using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour {

    void Update ()
    {
        gameObject.GetComponent<Renderer>().enabled = UI.ShowShipIds;
        if (UI.ShowShipIds)
        {
            transform.forward = GameObject.Find("CameraHolder/Main Camera").transform.forward;
        }
	}
}

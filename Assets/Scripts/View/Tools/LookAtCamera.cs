using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour {

	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            gameObject.GetComponent<Renderer>().enabled = true;
            transform.forward = GameObject.Find("CameraHolder/Main Camera").transform.forward;
        }

        if (Input.GetKeyUp(KeyCode.LeftAlt))
        {
            gameObject.GetComponent<Renderer>().enabled = false;
        }
	}
}

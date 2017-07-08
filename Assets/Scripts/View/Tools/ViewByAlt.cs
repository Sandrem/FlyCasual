using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewByAlt : MonoBehaviour {

	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            gameObject.GetComponent<Canvas>().enabled = true;
        }

        if (Input.GetKeyUp(KeyCode.LeftAlt))
        {
            gameObject.GetComponent<Canvas>().enabled = false;
        }
	}
}

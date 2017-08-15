using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionProjectorRotation : MonoBehaviour {

    const float ROTATION_SPEED = 20f;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (gameObject.activeSelf)
        {
            Vector3 currentAngles = transform.localEulerAngles;
            transform.localEulerAngles = new Vector3(currentAngles.x, currentAngles.y + Time.deltaTime*ROTATION_SPEED, currentAngles.z);
        }
	}
}

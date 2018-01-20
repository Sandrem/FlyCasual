using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MunitionMovement : MonoBehaviour {

    public float selfDescructTimer;
    public float animationSpeed = 1;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        float progress = Time.deltaTime * animationSpeed;
        transform.position = Vector3.MoveTowards(transform.position, transform.position +  transform.TransformVector(Vector3.forward), progress);

        selfDescructTimer -= Time.deltaTime * animationSpeed;
        if (selfDescructTimer <= 0) Destroy(this.gameObject);
	}
}

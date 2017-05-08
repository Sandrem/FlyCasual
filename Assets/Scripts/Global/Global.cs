using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global : MonoBehaviour {

    public static string test = "I am alive";

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(this.gameObject);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewByAlt : MonoBehaviour {

    void Update ()
    {
        gameObject.GetComponent<Canvas>().enabled = UI.ShowShipIds;
    }
}

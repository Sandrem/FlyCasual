using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsUI : MonoBehaviour {

    public GameObject PlaymatSelector;

    public void OnClickPlaymatChange(GameObject playmatImage)
    {
        Global.Playmat = playmatImage.name;
        PlaymatSelector.transform.position = playmatImage.transform.position;
    }

}

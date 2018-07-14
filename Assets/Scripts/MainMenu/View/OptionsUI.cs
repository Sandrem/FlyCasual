using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsUI : MonoBehaviour {

    public GameObject PlaymatSelector;

    public void OnClickPlaymatChange(GameObject playmatImage)
    {
        PlayerPrefs.SetString("PlaymatName", playmatImage.name);
        PlayerPrefs.Save();

        Options.Playmat = playmatImage.name;

        PlaymatSelector.transform.position = playmatImage.transform.position;
    }

    public void RestoreDefaults()
    {
        Options.Playmat = "3DSceneHoth";
        Options.ChangeParameterValue("Music Volume", 0.25f);
        Options.ChangeParameterValue("SFX Volume", 0.25f);
        Options.ChangeParameterValue("Animation Speed", 0.25f);
        Options.ChangeParameterValue("Maneuver Speed", 0.25f);

        Options.InitializePanel();
    }

}

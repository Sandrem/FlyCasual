using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mods;

public class ModUI : MonoBehaviour {

    public void OnClickModActivationChange(GameObject ModGO)
    {
        bool toggleValue = ModGO.transform.Find("Toggle").GetComponent<Toggle>().isOn;
        ModsManager.ModToggleIsActive(ModGO.name, toggleValue);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ExtraOptions;

public class ExtraOptionUI : MonoBehaviour
{
    public void OnClickModActivationChange(GameObject ExtraOptionGO)
    {
        bool toggleValue = ExtraOptionGO.transform.Find("Toggle").GetComponent<Toggle>().isOn;
        ExtraOptionsManager.ExtraOptionToggleIsActive(ExtraOptionGO.name, toggleValue);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsValueController : MonoBehaviour
{
    public void Start()
    {
        SetValue(PlayerPrefs.GetInt(this.transform.Find("Text").GetComponent<Text>().text, 4));
    }

    public void SetValue(int value)
    {
        Options.ChangeParameterValue(this.transform.Find("Text").GetComponent<Text>().text, value);
        PlayerPrefs.Save();

        int i = 1;
        foreach (Transform scalePanel in this.transform.Find("ValueList"))
        {
            scalePanel.GetComponent<Image>().color = (i <= value) ? Color.blue : new Color(1, 1, 1, 100f/255f);
            i++;
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsValueController : MonoBehaviour
{
    public void Start()
    {
        SetValue(PlayerPrefs.GetFloat(this.transform.Find("Text").GetComponent<Text>().text, 0.25f));
    }

    public void UpdateProgressByClick()
    {
        float percentage = (Input.mousePosition.x - this.gameObject.transform.position.x - 20) / 520;
        if (percentage < 0.05) percentage = 0;
        else if (percentage > 0.95) percentage = 1;

        SetValue(percentage);
    }

    public void SetValue(float percentage)
    {
        Options.ChangeParameterValue(this.transform.Find("Text").GetComponent<Text>().text, percentage);
        PlayerPrefs.Save();

        this.transform.Find("ValueList/PanelValue").GetComponent<RectTransform>().sizeDelta = new Vector2(520f * percentage, 50);
        this.transform.Find("ValueList/PanelEmpty").GetComponent<RectTransform>().sizeDelta = new Vector2(520f * (1f - percentage), 50);
    }

}

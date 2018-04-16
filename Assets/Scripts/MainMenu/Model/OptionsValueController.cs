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
        float UiScale = GameObject.Find("UI").GetComponent<RectTransform>().localScale.x;
        float percentage = (Input.mousePosition.x - this.gameObject.transform.position.x - (20f * UiScale)) / (595f * UiScale);

        string optionName = this.transform.Find("Text").GetComponent<Text>().text;
        if (optionName.Contains("Speed"))
        {
            if (percentage < 0.05f) percentage = 0.05f;
        }
        else
        {
            if (percentage < 0.05f) percentage = 0f;
        }

        if (percentage > 0.95f) percentage = 1f;

        SetValue(percentage);
    }

    public void SetValue(float percentage)
    {
        Options.ChangeParameterValue(this.transform.Find("Text").GetComponent<Text>().text, percentage);

        this.transform.Find("ValueList/PanelValue").GetComponent<RectTransform>().sizeDelta = new Vector2(595f * percentage, 50);
        this.transform.Find("ValueList/PanelEmpty").GetComponent<RectTransform>().sizeDelta = new Vector2(595f * (1f - percentage), 50);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsValueController : MonoBehaviour
{
    private float uiScale;
    public string OptionName;

    public void Start()
    {
        uiScale = GameObject.Find("UI").transform.localScale.x;
        SetValue(PlayerPrefs.GetFloat(OptionName, 0.25f));
    }

    public void UpdateProgressByClick()
    {
        float myWidth = this.transform.Find("PanelHitDetection").GetComponent<RectTransform>().rect.width * uiScale;
        float localCursorX = Input.mousePosition.x - this.transform.position.x;
        float percentage = (localCursorX + 0.5f * myWidth) / myWidth;

        Console.Write("MyWidth: " + myWidth + " LocCursorX: " + localCursorX + " Percent: " + percentage);

        if (OptionName.Contains("Speed"))
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
        Options.ChangeParameterValue(OptionName, percentage);

        float myWidth = this.transform.Find("PanelHitDetection").GetComponent<RectTransform>().rect.width;
        this.transform.Find("ValueList/PanelValue").GetComponent<RectTransform>().sizeDelta = new Vector2(myWidth * percentage, 100);
        this.transform.Find("ValueList/PanelEmpty").GetComponent<RectTransform>().sizeDelta = new Vector2(myWidth * (1f - percentage), 100);
    }

}

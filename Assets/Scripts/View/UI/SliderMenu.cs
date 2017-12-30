using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public static class SliderMenu
{
    public static GameObject SliderMenuGO;
    public static Slider SliderControl;

    public static void ShowSlider(float minValue, float maxValue, float defaultValue, UnityAction<float> onValueChanged)
    {
        GameObject prefab = (GameObject)Resources.Load("Prefabs/UI/SliderPanel", typeof(GameObject));
        SliderMenuGO = MonoBehaviour.Instantiate(prefab, GameObject.Find("UI").transform);

        SliderControl = SliderMenuGO.GetComponentInChildren<Slider>();
        SliderControl.minValue = minValue;
        SliderControl.maxValue = maxValue;
        SliderControl.value = defaultValue;

        SliderControl.onValueChanged.AddListener(onValueChanged);

        SliderMenuGO.SetActive(true);

        UI.ShowNextButton();
    }

    public static float GetSliderValue()
    {
        return SliderControl.value;
    }

    public static void CloseSlider()
    {
        SliderControl.onValueChanged.RemoveAllListeners();
        GameObject.Destroy(SliderMenuGO);
        UI.HideNextButton();
    }
}

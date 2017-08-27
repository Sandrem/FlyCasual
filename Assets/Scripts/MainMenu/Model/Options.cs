using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class Options
{
    private static OptionsUI optionsUI;

    public static void LoadOptions()
    {
        Global.Playmat = PlayerPrefs.GetString("PlaymatName", "Endor");
    }

    public static void InitializePanel()
    {
        optionsUI = GameObject.Find("UI/Panels/OptionsPanel").GetComponentInChildren<OptionsUI>();

        SetPlaymatOption();
    }

    private static void SetPlaymatOption()
    {
        foreach (Transform playmatImage in optionsUI.transform.Find("PlaymatsSelection/ImageList"))
        {
            if (playmatImage.name == Global.Playmat)
            {
                optionsUI.PlaymatSelector.transform.position = playmatImage.transform.position;
                break;
            }
        }
    }
}


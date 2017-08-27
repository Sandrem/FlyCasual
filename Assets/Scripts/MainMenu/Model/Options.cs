using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class Options
{
    private static OptionsUI optionsUI;

    public static string Playmat;
    public static int MusicVolume;

    static Options()
    {
        Playmat = PlayerPrefs.GetString("PlaymatName", "Endor");
        MusicVolume = PlayerPrefs.GetInt("Music Volume", 4);
    }

    public static void InitializePanel()
    {
        optionsUI = GameObject.Find("UI/Panels/OptionsPanel").GetComponentInChildren<OptionsUI>();

        SetPlaymatOption();
        UpdateVolume();
    }

    private static void SetPlaymatOption()
    {
        foreach (Transform playmatImage in optionsUI.transform.Find("PlaymatsSelection/ImageList"))
        {
            if (playmatImage.name == Playmat)
            {
                optionsUI.PlaymatSelector.transform.position = playmatImage.transform.position;
                break;
            }
        }
    }

    public static void ChangeParameterValue(string parameter, int value)
    {
        PlayerPrefs.SetInt(parameter, value);
        PlayerPrefs.Save();

        switch (parameter)
        {
            case "Music Volume":
                SetMusicVolume(value);
                break;
            default:
                break;
        }
    }

    public static void UpdateVolume()
    {
        SetMusicVolume(PlayerPrefs.GetInt("Music Volume", 4));
    }

    private static void SetMusicVolume(int value)
    {
        GameObject.Find("Music").GetComponent<AudioSource>().volume = value * 1f / 5f;
    }
}


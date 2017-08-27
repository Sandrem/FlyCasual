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
    public static int SfxVolume;
    public static int AnimationSpeed;
    public static int ManeuverSpeed;

    static Options()
    {
        Playmat = PlayerPrefs.GetString("PlaymatName", "Endor");
        MusicVolume = PlayerPrefs.GetInt("Music Volume", 4);
        SfxVolume = PlayerPrefs.GetInt("Sfx Volume", 4);
        AnimationSpeed = PlayerPrefs.GetInt("Animation Speed", 1);
        ManeuverSpeed = PlayerPrefs.GetInt("Maneuver Speed", 1);
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
                MusicVolume = value;
                SetMusicVolume(value);
                break;
            case "SFX Volume":
                SfxVolume = value;
                break;
            case "Animation Speed":
                AnimationSpeed = value;
                break;
            case "Maneuver Speed":
                ManeuverSpeed = value;
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


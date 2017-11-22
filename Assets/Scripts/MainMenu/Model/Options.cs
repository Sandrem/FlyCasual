using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Mods;

public static class Options
{
    private static OptionsUI optionsUI;

    public static string Playmat;
    public static string CheckVersionUrl;
    public static float MusicVolume;
    public static float SfxVolume;
    public static float AnimationSpeed;
    public static float ManeuverSpeed;

    static Options()
    {
        ReadOptions();
    }

    public static void ReadOptions()
    {
        Playmat = PlayerPrefs.GetString("PlaymatName", "Endor");
        CheckVersionUrl = PlayerPrefs.GetString("CheckVersionUrl", "http://sandrem.freeasphost.net/data/currentversion.txt");
        MusicVolume = PlayerPrefs.GetFloat("Music Volume", 0.25f);
        SfxVolume = PlayerPrefs.GetFloat("Sfx Volume", 0.25f);
        AnimationSpeed = PlayerPrefs.GetFloat("Animation Speed", 0.25f);
        ManeuverSpeed = PlayerPrefs.GetFloat("Maneuver Speed", 0.25f);

        ReadMods();
    }

    private static void ReadMods()
    {
        foreach (var modHolder in ModsManager.Mods)
        {
            modHolder.Value.IsOn = PlayerPrefs.GetInt("mods/" + modHolder.Key.ToString(), 0) == 1;
        }
    }

    public static void InitializePanel()
    {
        optionsUI = GameObject.Find("UI/Panels/OptionsPanel").GetComponentInChildren<OptionsUI>();

        SetPlaymatOption();
        SetValueControllers();
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

    private static void SetValueControllers()
    {
        foreach (OptionsValueController valueController in GameObject.Find("UI/Panels/OptionsPanel").GetComponentsInChildren<OptionsValueController>())
        {
            valueController.Start();
        }
    }

    public static void ChangeParameterValue(string parameter, float value)
    {
        PlayerPrefs.SetFloat(parameter, value);
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
        SetMusicVolume(PlayerPrefs.GetFloat("Music Volume", 0.25f));
    }

    private static void SetMusicVolume(float value)
    {
        GameObject.Find("Music").GetComponent<AudioSource>().volume = value * 0.2f;
    }

    public static void SetCheckVersionUrl(string newUrl)
    {
        PlayerPrefs.SetString("CheckVersionUrl", newUrl);
        CheckVersionUrl = newUrl;
    }
}


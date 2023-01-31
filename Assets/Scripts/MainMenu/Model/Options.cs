﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Mods;
using ExtraOptions;

public static class Options
{
    private static OptionsUI optionsUI;

    public static string Playmat;
    public static string BackgroundImage;
    public static string CheckVersionUrl;
    public static float MusicVolume;
    public static float SfxVolume;
    public static float AnimationSpeed;
    public static float ManeuverSpeed;
    public static string Avatar;
    public static string NickName;
    public static string Title;
    public static string Edition;
    public static bool DontShowAiInfo;
    public static string AiType;
    public static string DiceStats;
    public static bool FullScreen;
    public static bool ShowFps;
    public static int Quality;
    public static string Resolution;
    public static int DisplayId;

    public static readonly string DefaultAvatar = "UpgradesList.SecondEdition.AgileGunner";

    static Options()
    {
        ReadOptions();
    }

    public static void ReadOptions()
    {
        Playmat = PlayerPrefs.GetString("PlaymatName", "Endor");
        BackgroundImage = PlayerPrefs.GetString("BackgroundImage", "_RANDOM");
        CheckVersionUrl = PlayerPrefs.GetString("CheckVersionUrl", "http://sandrem.freeasphost.net/data/currentversion.txt");
        MusicVolume = PlayerPrefs.GetFloat("Music Volume", 0.25f);
        SfxVolume = PlayerPrefs.GetFloat("SFX Volume", 0.25f);
        AnimationSpeed = PlayerPrefs.GetFloat("Animation Speed V2", 0.25f);
        ManeuverSpeed = PlayerPrefs.GetFloat("Maneuver Speed V2", 0.25f);
        Avatar = PlayerPrefs.GetString("AvatarV2", Options.DefaultAvatar);
        NickName = PlayerPrefs.GetString("NickName", "Unknown Pilot");
        Title = PlayerPrefs.GetString("Title", "Test Pilot");
        DontShowAiInfo = PlayerPrefs.GetInt("DontShowAiInfo", 0) == 1;
        AiType = PlayerPrefs.GetString("AiType", "AI: Aggressor");
        Edition = "SecondEdition";
        ShowFps = PlayerPrefs.GetInt("ShowFps", 0) == 1;
        Resolution = PlayerPrefs.GetString("Resolution", Screen.currentResolution.ToString());

        FullScreen = PlayerPrefs.GetInt("FullScreen", 1) == 1;
        Screen.fullScreen = FullScreen;

        DisplayId = PlayerPrefs.GetInt("DisplayId", 0);
        if (DisplayId < 0 || DisplayId >= Display.displays.Count())
        {
            DisplayId = 0;
            ChangeParameterValue("DisplayId", 0);
        }

        Quality = PlayerPrefs.GetInt("Quality", 2);
        if (Quality > 2) Quality = 2;

        DiceStats = PlayerPrefs.GetString("DiceStats", "AT-0|AC-0|AS-0|AE-0|AB-0|DT-0|DS-0|DE-0|DB-0&AT-0|AC-0|AS-0|AE-0|AB-0|DT-0|DS-0|DE-0|DB-0");
        DiceStatsTracker.ReadFromString(DiceStats);

        MainMenu.SetEdition(Edition);

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
            case "Animation Speed V2":
                AnimationSpeed = value;
                break;
            case "Maneuver Speed V2":
                ManeuverSpeed = value;
                break;
            default:
                break;
        }
    }

    public static void ChangeParameterValue(string parameter, string value)
    {
        PlayerPrefs.SetString(parameter, value);
        PlayerPrefs.Save();
    }

    public static void ChangeParameterValue(string parameter, bool value)
    {
        PlayerPrefs.SetInt(parameter, (value) ? 1 : 0);
        PlayerPrefs.Save();
    }

    public static void ChangeParameterValue(string parameter, int value)
    {
        PlayerPrefs.SetInt(parameter, value);
        PlayerPrefs.Save();
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


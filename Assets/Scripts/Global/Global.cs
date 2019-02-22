﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Analytics;

public class Global : MonoBehaviour {

    public static Global Instance;

    private static bool isAlreadyInitialized;

    public static string CurrentVersion = "0.7.4";
    public static int CurrentVersionInt = 100070400;

    void Awake()
    {
        if (!isAlreadyInitialized)
        {
            Instance = this;
            isAlreadyInitialized = true;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Update()
    {
        Tooltips.CheckTooltip();
    }

    public static void BattleIsReady()
    {
        if ((Roster.Player2 is Players.HotacAiPlayer) && (!Options.DontShowAiInfo))
        {
            MainMenu.ShowAiInformationContinue();
        }
        else
        {
            StartBattle();
        }
    }

    public static void StartBattle()
    {
        if (DebugManager.ReleaseVersion)
        {
            AnalyticsEvent.GameStart(new Dictionary<string, object>()
            {
                { "Edition", Editions.Edition.Current.Name },
                { "GameMode", GameModes.GameMode.CurrentGameMode.Name }
            });
        }

        ToggelLoadingScreen(false);
        Phases.StartPhases();
    }

    public static void ToggelLoadingScreen(bool isActive)
    {
        Transform loadingScreen = GameObject.Find("GlobalUI").transform.Find("OpponentSquad");
        if (isActive) loadingScreen.GetComponent<Image>().sprite = MainMenu.GetRandomSplashScreen();
        if (loadingScreen != null) loadingScreen.gameObject.SetActive(isActive);
        if (isActive) MainMenu.ResetAiInformation();
    }

    public static Scene ActiveScene
    {
        get
        {
            switch (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name)
            {
                case "MainMenu":
                    return Scene.MainMenu;
                case "Battle":
                    return Scene.Battle;
                default:
                    return Scene.Undefined;
            }
        }
    }

    public enum Scene
    {
        Undefined,
        MainMenu,
        Battle
    }

}

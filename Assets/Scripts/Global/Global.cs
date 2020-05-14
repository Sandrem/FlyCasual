using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public class Global : MonoBehaviour {

    public static Global Instance;

    private static bool isAlreadyInitialized;

    public static string CurrentVersion = "1.0.1";
    public static int CurrentVersionInt = 101000101;
    public static int LatestVersionInt = 101000101;

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
            MainMenu.ShowLoadingScreenNetworkContinue();
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
                { "GameMode", GameModes.GameMode.CurrentGameMode.Name },
                { "Version", Global.CurrentVersion }
            });
        }

        LoadingScreen.NextSceneIsReady(Phases.StartPhases);
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

    public static void ReturnToMainMenu()
    {
        Phases.EndGame();
        LoadingScreen.Show();
        SceneManager.LoadScene("MainMenu");
        LoadingScreen.NextSceneIsReady(delegate { });
    }

    public static void ShowAnotherPlayerDisconnected()
    {
        UI.ShowGameResults(Roster.GetOpponent().NickName + " is disconnected");
    }

}

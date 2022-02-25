using GameModes;
using Players;
using SquadBuilderNS;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public class Global : MonoBehaviour {

    public static Global Instance;

    private static bool isAlreadyInitialized;

    public static string CurrentVersion = "2.0";
    public static int CurrentVersionInt = 102000000;
    public static int LatestVersionInt  = 102000000;

    public static SquadBuilder SquadBuilder { get; set;}

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

        SquadBuilder.Instance.Database.ClearData();
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

    public static void StartNetworkGame()
    {
        GameController.Initialize();
        ReplaysManager.TryInitialize(ReplaysMode.Write);

        GameMode.CurrentGameMode = new NetworkGame();
        SwitchToBattleScene();
    }

    public static void StartLocalGame()
    {
        GameMode.CurrentGameMode = new LocalGame();
        SwitchToBattleScene();
    }

    public static void SwitchToBattleScene()
    {
        LoadingScreen.Show();
        LoadBattleScene();
    }

    private static void LoadBattleScene()
    {
        SceneManager.LoadScene("Battle");
    }

    public static bool IsVsNetworkOpponent
    {
        get { return SquadBuilder.SquadLists[PlayerNo.Player2].PlayerType == typeof(NetworkOpponentPlayer); }
    }

    public static bool IsNetworkGame
    {
        get { return SquadBuilder.SquadLists[PlayerNo.Player2].PlayerType == typeof(NetworkOpponentPlayer) 
                || SquadBuilder.SquadLists[PlayerNo.Player1].PlayerType == typeof(NetworkOpponentPlayer); }
    }

    public static PlayerNo MyPlayer
    {
        get {
            if (IsNetworkGame)
            {
                if (SquadBuilder.SquadLists[PlayerNo.Player2].PlayerType == typeof(NetworkOpponentPlayer)) return PlayerNo.Player1;
                if (SquadBuilder.SquadLists[PlayerNo.Player1].PlayerType == typeof(NetworkOpponentPlayer)) return PlayerNo.Player2;
            }

            return PlayerNo.Player1;
        }
    }

    public static bool IsVsAiGame
    {
        get { return SquadBuilder.SquadLists[PlayerNo.Player2].PlayerType.IsSubclassOf(typeof(GenericAiPlayer)); }
    }

    public static void PrepareOnlineMatchLists(int playerInt, string playerName, string title, string avatar, string squadString)
    {
        PlayerNo playerNo = Tools.IntToPlayer(playerInt);
        SquadList squadList = SquadBuilder.SquadLists[playerNo];

        if (Network.IsServer)
        {
            squadList.PlayerType = (playerNo == PlayerNo.Player1) ? typeof(HumanPlayer) : typeof(NetworkOpponentPlayer);
        }
        else
        {
            squadList.PlayerType = (playerNo == PlayerNo.Player1) ? typeof(NetworkOpponentPlayer) : typeof(HumanPlayer);
        }

        SquadBuilder.SquadLists[playerNo].CreateSquadFromImportedJson(squadString);

        squadList.SavedConfiguration = SquadBuilder.SquadLists[playerNo].GetSquadInJson();

        JSONObject playerInfoJson = new JSONObject();
        playerInfoJson.AddField("NickName", playerName);
        playerInfoJson.AddField("Title", title);
        playerInfoJson.AddField("Avatar", avatar);
        squadList.SavedConfiguration.AddField("PlayerInfo", playerInfoJson);
    }

}

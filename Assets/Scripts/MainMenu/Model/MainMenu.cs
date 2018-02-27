using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mods;
using UnityEngine.UI;
using SquadBuilderNS;
using System.Linq;
using Players;

public partial class MainMenu : MonoBehaviour {

    string NewVersionUrl;

    public static MainMenu CurrentMainMenu;

    // Use this for initialization
    void Start ()
    {
        InitializeMenu();
    }

    private void InitializeMenu()
    {
        CurrentMainMenu = this;

        SetCurrentPanel();

        DontDestroyOnLoad(GameObject.Find("GlobalUI").gameObject);

        ModsManager.Initialize();
        Options.ReadOptions();
        Options.UpdateVolume();
        CheckUpdates();
    }

    public void StartBattle()
    {
        if (SquadBuilder.ValidateCurrentPlayersRoster())
        {
            SquadBuilder.SaveSquadConfigurations();
            ShipFactory.Initialize();

            if (!SquadBuilder.IsNetworkGame)
            {
                SquadBuilder.StartLocalGame();
            }
            else
            {
                ChangePanel("MultiplayerDecisionPanel");
            }
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OnUpdateAvailableClick()
    {
        Application.OpenURL(NewVersionUrl);
    }

    private void CheckUpdates()
    {
        int latestVersionInt = RemoteSettings.GetInt("UpdateLatestVersionInt", Global.CurrentVersionInt);
        if (latestVersionInt > Global.CurrentVersionInt)
        {
            string latestVersion    = RemoteSettings.GetString("UpdateLatestVersion", Global.CurrentVersion);
            string updateLink       = RemoteSettings.GetString("UpdateLink");
            ShowNewVersionIsAvailable(latestVersion, updateLink);
        }
    }

    // 0.3.2 UI

    public void CreateMatch()
    {
        string roomName = GameObject.Find("UI/Panels/CreateMatchPanel/Panel/Name").GetComponentInChildren<InputField>().text;
        string password = GameObject.Find("UI/Panels/CreateMatchPanel/Panel/Password").GetComponentInChildren<InputField>().text;
        Network.CreateMatch(roomName, password);
    }

    public void BrowseMatches()
    {
        Network.BrowseMatches();
    }

    public void JoinMatch(GameObject panel)
    {
        //Messages.ShowInfo("Joining room...");
        string password = panel.transform.Find("Password").GetComponentInChildren<InputField>().text;
        Network.JoinCurrentRoomByParameters(password);
    }

    public void CancelWaitingForOpponent()
    {
        Network.CancelWaitingForOpponent();
    }

    public void StartSquadBuilerMode(string modeName)
    {
        SquadBuilder.Initialize();
        SquadBuilder.SetCurrentPlayer(PlayerNo.Player1);
        SquadBuilder.SetPlayers(modeName);
        SquadBuilder.SetDefaultPlayerNames();
        ChangePanel("SelectFactionPanel");
    }

}

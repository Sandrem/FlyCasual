using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mods;
using UnityEngine.UI;
using SquadBuilderNS;
using System.Linq;

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
        StartCoroutine(CheckUpdates());
    }

    public void StartBattle()
    {
        ShipFactory.Initialize();

        if (!IsNetworkGame())
        {
            SquadBuilder.StartLocalGame();
        }
        else
        {
            GameObject squadBuilerPanel = GameObject.Find("UI/Panels").transform.Find("MultiplayerDecisionPanel").gameObject;
            ChangePanel(squadBuilerPanel);
        }
    }

    private bool IsNetworkGame()
    {
        return SquadBuilder.SquadLists.Find(n => n.PlayerNo == Players.PlayerNo.Player2).PlayerType == typeof(Players.NetworkOpponentPlayer);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OnUpdateAvailableClick()
    {
        Application.OpenURL(NewVersionUrl);
    }

    private IEnumerator CheckUpdates()
    {
        WWW www = new WWW(Options.CheckVersionUrl);
        yield return www;

        string[] separator = new string[] { "\r\n" };
        string[] wwwdata = www.text.Split(separator, System.StringSplitOptions.RemoveEmptyEntries);

        if (wwwdata.Length > 0 && !wwwdata[0].Contains("DOCTYPE"))
        {
            if (wwwdata.Length == 3)
            {
                Options.SetCheckVersionUrl(wwwdata[2]);
                StartCoroutine(CheckUpdates());
            }
            else
            {
                if (wwwdata.Length == 2)
                {
                    if (wwwdata[0] != Global.CurrentVersion)
                    {
                        ShowNewVersionIsAvailable(wwwdata[0], wwwdata[1]);
                    }
                }
            }
        }
    }

    /*public void ImportSquadList()
    {
        RosterBuilder.ImportSquadList();
    }

    public void ExportSquadList()
    {
        RosterBuilder.ExportSquadList(Players.PlayerNo.Player1);
    }*/

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
        SquadBuilder.SetPlayers(modeName);
        ChangePanel("SelectFactionPanel");
    }

}

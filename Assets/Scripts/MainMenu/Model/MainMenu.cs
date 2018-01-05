using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mods;
using UnityEngine.UI;

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
        if (!IsNetworkGame())
        {
            RosterBuilder.StartLocalGame();
        }
        else
        {
            GameObject squadBuilerPanel = GameObject.Find("UI/Panels").transform.Find("MultiplayerDecisionPanel").gameObject;
            ChangePanel(squadBuilerPanel);
        }
    }

    private bool IsNetworkGame()
    {
        return GameObject.Find("UI/Panels/RosterBuilderPanel/PlayersPanel/Player2Panel/GroupPlayer").GetComponentInChildren<Dropdown>().value == 2;
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

    public void ImportSquadList()
    {
        RosterBuilder.ImportSquadList();
    }

    public void ExportSquadList()
    {
        RosterBuilder.ExportSquadList(Players.PlayerNo.Player1);
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
        GameObject squadBuilerPanel = GameObject.Find("UI/Panels").transform.Find("RosterBuilderPanel").gameObject;
        ChangePanel(squadBuilerPanel);

        bool hideSecondPart = false;
        int player1type = 0;
        int player2type = 0;

        switch (modeName)
        {
            case "vsAI":
                player2type = 1;
                break;
            case "Internet":
                hideSecondPart = true;
                player2type = 2;
                break;
            case "HotSeat":
                break;
            case "AIvsAI":
                player1type = 1;
                player2type = 1;
                break;
            default:
                break;
        }

        GameObject.Find("UI/Panels/RosterBuilderPanel/PlayersPanel/Player1Panel/GroupPlayer").GetComponentInChildren<Dropdown>().value = player1type;
        GameObject.Find("UI/Panels/RosterBuilderPanel/PlayersPanel/Player2Panel/GroupPlayer").GetComponentInChildren<Dropdown>().value = player2type;

        GameObject.Find("UI/Panels/RosterBuilderPanel/DisableSecondPlayer").SetActive(hideSecondPart);
    }

}

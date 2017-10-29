using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameModes;

public partial class MainMenu : MonoBehaviour {

    string NewVersionUrl;

    public static MainMenu CurrentMainMenu;

    // Use this for initialization
    void Start () {
        InitializeMenu();
    }

    private void InitializeMenu()
    {
        CurrentMainMenu = this;

        SetPositions();
        SetCurrentPanel();

        Options.ReadOptions();
        Options.UpdateVolume();
        StartCoroutine(CheckUpdates());
    }

    public void StartBattle()
    {
        if (!Network.IsNetworkGame)
        {
            GameMode.CurrentGameMode = new LocalGame();
        }
        else
        {
            GameMode.CurrentGameMode = new NetworkGame();
        }
        
        GameMode.CurrentGameMode.StartGame();
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
                if (wwwdata[0] != Global.CurrentVersion)
                {
                    ShowNewVersionIsAvailable(wwwdata[0], wwwdata[1]);
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

}

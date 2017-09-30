﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class MainMenu : MonoBehaviour {

    string NewVersionUrl;

    // Use this for initialization
    void Start () {
        InitializeMenu();
    }

    private void InitializeMenu()
    {
        SetPositions();
        SetCurrentPanel();

        Options.UpdateVolume();
        StartCoroutine(CheckUpdates());
    }

    public void StartBattle()
    {
        RosterBuilder.StartGame();
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

        if (wwwdata.Length > 0)
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

}

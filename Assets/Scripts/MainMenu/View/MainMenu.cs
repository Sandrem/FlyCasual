﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Reflection;
using System.Linq;
using Players;
using Mods;
using SquadBuilderNS;

public partial class MainMenu : MonoBehaviour {

    public GameObject RosterBuilderPrefab;
    public GameObject UpgradeLinePrefab;

    public GameObject CurrentPanel;

    private void SetCurrentPanel()
    {
        CurrentPanel = GameObject.Find("UI/Panels/MainMenuPanel");
    }

    public void ChangePanel(GameObject panel)
    {
        CurrentPanel.SetActive(false);
        InitializePanelContent(panel.name, CurrentPanel.name);
        panel.SetActive(true);
        CurrentPanel = panel;
    }

    public void ChangePanel(string panelName)
    {
        GameObject panel = GameObject.Find("UI/Panels").transform.Find(panelName).gameObject;
        ChangePanel(panel);
    }

    private void InitializePanelContent(string panelName, string previousPanelName)
    {
        switch (panelName)
        {
            case "RosterBuilderPanel":
                if (previousPanelName == "GameModeDecisionPanel") RosterBuilder.Initialize();
                break;
            case "OptionsPanel":
                Options.InitializePanel();
                break;
            case "ModsPanel":
                ModsManager.InitializePanel();
                break;
            case "BrowseRoomsPanel":
                Network.BrowseMatches();
                break;
            case "SelectFactionPanel":
                SquadBuilder.Initialize();
                break;
            case "SquadBuilderPanel":
                break;
            case "SelectShipPanel":
                SquadBuilder.ShowShipsFilteredByFaction();
                break;
        }
    }

    private void ShowNewVersionIsAvailable(string newVersion, string downloadUrl)
    {
        GameObject panel = GameObject.Find("UI/Panels").transform.Find("MainMenuPanel").Find("NewVersionIsAvailable").gameObject;

        panel.transform.Find("Text").GetComponent<Text>().text = "New version\nis available!\n\n" + newVersion;
        panel.transform.position = new Vector2(Screen.width - 20,  20);
        NewVersionUrl = downloadUrl;

        panel.SetActive(true);
    }

}

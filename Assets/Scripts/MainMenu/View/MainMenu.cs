using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Reflection;
using System.Linq;
using Players;
using Mods;
using SquadBuilderNS;
using RuleSets;

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
        if (RuleSet.Instance.IsSquadBuilderLocked)
        {
            if (panel.name == "SquadronOptionsPanel")
            {
                Messages.ShowError("Squad building is disabled");
                return;
            }
        }

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
            case "MainMenuPanel":
                UpdatePlayerInfo();
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
                SquadBuilder.ClearShipsOfPlayer(SquadBuilder.CurrentPlayer);
                break;
            case "SquadBuilderPanel":
                SquadBuilder.UpdateSquadName("SquadBuilderPanel");
                SquadBuilder.ShowShipsAndUpgrades();
                SquadBuilder.UpdateNextButton();
                break;
            case "SelectShipPanel":
                SquadBuilder.ShowShipsFilteredByFaction();
                break;
            case "SelectPilotPanel":
                SquadBuilder.ShowPilotsFilteredByShipAndFaction();
                break;
            case "ShipSlotsPanel":
                SquadBuilder.ShowPilotWithSlots();
                break;
            case "SelectUpgradePanel":
                SquadBuilder.ShowUpgradesList();
                break;
            case "SquadronOptionsPanel":
                SquadBuilder.UpdateSquadName("SquadronOptionsPanel");
                break;
            case "BrowseSavedSquadsPanel":
                SquadBuilder.BrowseSavedSquads();
                break;
            case "SaveSquadronPanel":
                SquadBuilder.PrepareSaveSquadronPanel();
                break;
            case "ShipSkinsPanel":
                SquadBuilder.ShowSkinButtons();
                break;
            case "AvatarsPanel":
                InitializePlayerCustomization();
                break;
            case "EditionPanel":
                ShowActiveEdition(Options.Edition);
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

    public void ChangeEditionIsClicked(GameObject editionGO)
    {
        ShowActiveEdition(editionGO.name);
        SetEdition(editionGO.name);
    }

    private void ShowActiveEdition(string editionName)
    {
        foreach (Transform panelTransform in GameObject.Find("UI/Panels/EditionPanel/Content").gameObject.transform)
        {
            Image backgroundImage = panelTransform.GetComponent<Image>();
            if (backgroundImage != null) backgroundImage.enabled = false;
        }

        GameObject.Find("UI/Panels/EditionPanel/Content/" + editionName).GetComponent<Image>().enabled = true;
    }

    public static void SetEdition(string editionName)
    {
        Options.ChangeParameterValue("Edition", editionName);

        switch (editionName)
        {
            case "FirstEdition":
                new RuleSets.FirstEdition();
                break;
            case "SecondEdition":
                new RuleSets.SecondEdition();
                break;
            default:
                break;
        }
    }

}

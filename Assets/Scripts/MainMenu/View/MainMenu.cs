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
using Editions;
using Ship;

public partial class MainMenu : MonoBehaviour {

    public GameObject RosterBuilderPrefab;
    public GameObject UpgradeLinePrefab;

    public GameObject CurrentPanel;
    public string PreviousPanelName;

    private void SetCurrentPanel()
    {
        CurrentPanel = GameObject.Find("UI/Panels/MainMenuPanel");
    }

    public void ChangePanel(GameObject panel)
    {
        ChangePanel(panel.name);
    }

    public void ChangePanel(string panelName)
    {
        PreviousPanelName = CurrentPanel.name;

        if (Edition.Current.IsSquadBuilderLocked)
        {
            if (panelName == "SquadronOptionsPanel")
            {
                if (CurrentPanel.name == "SquadBuilderPanel")
                {
                    Messages.ShowError("This part of squad builder is disabled");
                    return;
                }
                else
                {
                    panelName = "SelectFactionPanel";
                }
            }
        }

        CurrentPanel.SetActive(false);

        GameObject panel = GameObject.Find("UI/Panels").transform.Find(panelName).gameObject;
        InitializePanelContent(panelName, CurrentPanel.name);
        panel.SetActive(true);
        CurrentPanel = panel;
    }

    private void InitializePanelContent(string panelName, string previousPanelName)
    {
        switch (panelName)
        {
            case "MainMenuPanel":
                UpdatePlayerInfo();
                break;
            case "OptionsPanel":
                //Options.InitializePanel();
                OptionsUI.Instance.InitializeOptionsPanel();
                break;
            case "ModsPanel":
                ModsManager.InitializePanel();
                break;
            case "CreditsPanel":
                CreditsUI.InitializePanel();
                break;
            case "BrowseRoomsPanel":
                Network.BrowseMatches();
                break;
            case "SelectFactionPanel":
                SquadBuilder.SetCurrentPlayerFaction(Faction.None);
                SquadBuilder.ClearShipsOfPlayer(SquadBuilder.CurrentPlayer);
                SquadBuilder.ShowFactionsImages();
                break;
            case "SquadBuilderPanel":
                SquadBuilder.CheckAiButtonVisibility();
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
            case "ShipInfoPanel":
                SquadBuilder.ShowShipInformation();
                break;
            case "SkinsPanel":
                SquadBuilder.ShowSkinsPanel();
                break;
        }
    }

    private void ShowNewVersionIsAvailable(string newVersion, string downloadUrl)
    {
        GameObject mainMenuPanel = GameObject.Find("UI/Panels").transform.Find("MainMenuPanel").gameObject;
        if (!mainMenuPanel.activeSelf) return;

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
        Options.Edition = editionName;
        Options.ChangeParameterValue("Edition", editionName);

        switch (editionName)
        {
            case "FirstEdition":
                new FirstEdition();
                break;
            case "SecondEdition":
                new SecondEdition();
                break;
            default:
                break;
        }
    }

    public void PreviousPanel()
    {
        CurrentMainMenu.ChangePanel(CurrentMainMenu.PreviousPanelName);
    }

}

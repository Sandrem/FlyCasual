using UnityEngine;
using UnityEngine.UI;
using Mods;
using SquadBuilderNS;
using Editions;

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
                ClearBatchAiSquadsTestingMode();
                break;
            case "OptionsPanel":
                OptionsUI.Instance.InitializeOptionsPanel();
                break;
            case "StatsPanel":
                StatsUI.Instance.InitializeStatsPanel();
                break;
            case "ModsPanel":
                ModsManager.InitializePanel();
                break;
            case "CreditsPanel":
                CreditsUI.InitializePanel();
                break;
            case "BrowseRoomsPanel":
                BrowseMatches();
                break;
            case "SelectFactionPanel":
                Global.SquadBuilder.CurrentSquad.ClearAll();
                Global.SquadBuilder.View.ShowFactionsImages();
                break;
            case "SquadBuilderPanel":
                Global.SquadBuilder.View.ShowShipsAndUpgrades();
                Global.SquadBuilder.View.UpdateNextButton();
                break;
            case "SelectShipPanel":
                Global.SquadBuilder.View.ShowShipsFilteredByFaction();
                break;
            case "SelectPilotPanel":
                Global.SquadBuilder.View.ShowPilotsFilteredByShipAndFaction();
                break;
            case "ShipSlotsPanel":
                Global.SquadBuilder.View.ShowPilotWithSlots();
                break;
            case "SelectUpgradePanel":
                Global.SquadBuilder.View.ShowUpgradesList();
                break;
            case "BrowseSavedSquadsPanel":
                Global.SquadBuilder.View.BrowseSavedSquads();
                break;
            case "SaveSquadronPanel":
                Global.SquadBuilder.View.PrepareSaveSquadronPanel();
                break;
            case "AvatarsPanel":
                InitializePlayerCustomization();
                break;
            case "EditionPanel":
                ShowActiveEdition(Options.Edition);
                break;
            case "ShipInfoPanel":
                Global.SquadBuilder.View.ShowShipInformation();
                break;
            case "SkinsPanel":
                Global.SquadBuilder.View.ShowSkinsPanel();
                break;
            case "ChosenObstaclesPanel":
                Global.SquadBuilder.View.ShowChosenObstaclesPanel();
                break;
            case "BrowseObstaclesPanel":
                Global.SquadBuilder.View.ShowBrowseObstaclesPanel();
                break;
            case "BrowsePopularSquadsPanel":
                if (previousPanelName == "SquadOptionsPanel") PopularSquads.LastChosenFaction = "All";
                PopularSquads.LoadPopularSquads();
                break;
            case "BrowsePopularSquadsVariantsPanel":
                PopularSquads.LoadPopularSquadsVariants();
                break;
            case "BrowseAvatarsPanel":
                AvatarsManager.LoadAvatars(Faction.None);
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

    private void ShowSupportOnPatreon(int support)
    {
        //Don't show if new version is available
        if (Global.LatestVersionInt <= Global.CurrentVersionInt)
        {
            GameObject mainMenuPanel = GameObject.Find("UI/Panels").transform.Find("MainMenuPanel").gameObject;
            if (!mainMenuPanel.activeSelf) return;

            GameObject panel = GameObject.Find("UI/Panels").transform.Find("MainMenuPanel").Find("SupportOnPatreon").gameObject;

            panel.transform.Find("Text").GetComponent<Text>().text = $"\"Rules 2.5\" and new expansions are coming! If you want\nto see them in Fly Casual -\nsupport me on patreon\n{support} / 200";
            panel.transform.position = new Vector2(Screen.width - 20, 20);

            panel.SetActive(true);
        }        
    }

    private void ShowSupportUkraine()
    {
        GameObject mainMenuPanel = GameObject.Find("UI/Panels").transform.Find("MainMenuPanel").gameObject;
        if (!mainMenuPanel.activeSelf) return;

        GameObject panel = GameObject.Find("UI/Panels").transform.Find("MainMenuPanel").Find("SupportUkraine").gameObject;
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
            /*case "FirstEdition":
                new FirstEdition();
                break;*/
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

    public void OpenPatreon()
    {
        Application.OpenURL("https://www.patreon.com/Sandrem");
    }

    public void SetFaction(string factionChar)
    {
        PopularSquads.SetFaction(factionChar);
    }

    public void SetFactionForAvatars(string factionChar)
    {
        AvatarsManager.LoadAvatars(factionChar);
    }

}
